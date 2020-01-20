using Fap.AspNetCore.ViewModel;
using Fap.Core.DataAccess;
using Fap.Core.DI;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.Infrastructure.Enums;
using Fap.Core.Rbac.Model;
using Fap.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ardalis.GuardClauses;
using Fap.AspNetCore.Controls;

namespace Fap.Hcm.Service.Organization
{
    [Service]
    public class OrganizationService : IOrganizationService
    {
        private readonly IDbContext _dbContext;
        private readonly IFapPlatformDomain _platformDomain;
        public OrganizationService(IDbContext dbContext, IFapPlatformDomain platformDomain)
        {
            _dbContext = dbContext;
            _platformDomain = platformDomain;
        }
        [Transactional]
        public ResponseViewModel MoveDepartment(TreePostData postData)
        {
            Guard.Against.Null(postData, nameof(postData));
            if (postData.Operation == TreeNodeOper.MOVE_NODE)
            {
                //当前部门
                _platformDomain.OrgDeptSet.TryGetValue(postData.Id, out OrgDept currDept);

                //检查之前父部门子级
                _platformDomain.OrgDeptSet.TryGetValueByPid(currDept.Pid, out IEnumerable<OrgDept> childs);
                if (childs.Any() && childs.Count() == 1)
                {
                    if (_platformDomain.OrgDeptSet.TryGetValue(currDept.Pid, out OrgDept parentDept))
                    {
                        parentDept.IsFinal = 1;
                        _dbContext.Update(parentDept);
                    }
                }
                currDept.Pid = postData.Parent;
                _dbContext.Update(currDept);
            }
            return new ResponseViewModel { success = true };
        }
        [Transactional]
        public ResponseViewModel MergeDepartment(MergeDeptModel mergeDept)
        {
            Guard.Against.Null(mergeDept, nameof(mergeDept));
            try
            {
                var employees = _dbContext.QueryWhere<Employee>("DeptUid in @Depts", new Dapper.DynamicParameters(new { Depts = mergeDept.MergeFids }));
                foreach (var employee in employees)
                {
                    employee.DeptUid = mergeDept.DeptFid;
                    employee.DeptCode = mergeDept.DeptCode;
                    _dbContext.Update(employee);
                }
                //删除旧部门
                var mergeDepts = _platformDomain.OrgDeptSet.Where(d => mergeDept.MergeFids.Contains(d.Fid));
                foreach (var dept in mergeDepts)
                {
                    _dbContext.Delete(dept);
                }
                return ResponseViewModelUtils.Sueecss();

            }
            catch (Exception ex)
            {
                //删除新建的合并部门
                OrgDept dept = _dbContext.Get<OrgDept>(mergeDept.DeptFid);
                _dbContext.Delete(dept);
                return ResponseViewModelUtils.Failure(ex.Message);
            }
        }

        public List<TreeDataView> GetOrgJobTree()
        {
            IEnumerable<OrgJob> jobGroups = _dbContext.QueryAll<OrgJob>();
            List<TreeDataView> oriList = jobGroups.Select(t => new TreeDataView { Id = t.Fid, Pid = t.Pid, Data = new { group = "1",isfinal=t.IsFinal,fullname=t.FullName }, Text = t.JobName, Icon = "icon-folder orange ace-icon fa fa-users" }).ToList<TreeDataView>();
            List<TreeDataView> tree = new List<TreeDataView>();
            TreeDataView treeRoot = new TreeDataView()
            {
                Id = "0",
                Text = "职位层级",
                Data = new { group = "0", isfinal=0,fullname="" },
                State = new NodeState { Opened = true },
                Icon = "icon-folder blue ace-icon fa fa-sitemap",
            };
            tree.Add(treeRoot);
            TreeViewHelper.MakeTree(treeRoot.Children, oriList, treeRoot.Id);
            return tree;
        }

        public ResponseViewModel OperOrgJob(TreePostData postData)
        {
            Guard.Against.Null(postData,nameof(postData));
            ResponseViewModel vm = new ResponseViewModel();
            if (postData.Operation == TreeNodeOper.DELETE_NODE)
            {
                vm.success = _dbContext.Delete<OrgJob>(postData.Id);
            }
            else if (postData.Operation == TreeNodeOper.CREATE_NODE)
            {
                string pid = postData.Id;
                var parentJob= _dbContext.Get<OrgJob>(pid);
                OrgJob job = new OrgJob()
                {
                    Pid = postData.Id,
                    JobName = postData.Text,
                    TreeLevel = parentJob == null ? 1 : parentJob.TreeLevel + 1,
                    IsFinal = parentJob == null ? 0 : 1,
                    FullName=parentJob==null?postData.Text:$"{parentJob.FullName}/{postData.Text}"
                };
                if (parentJob != null)
                {
                    parentJob.IsFinal = 0;
                    _dbContext.Update(parentJob);
                }
                _dbContext.Insert(job);
                vm.success = true;
                vm.data = job.Fid;
            }
            else if (postData.Operation == TreeNodeOper.RENAME_NODE)
            {
                var job = _dbContext.Get<OrgJob>(postData.Id);
                job.FullName = job.FullName.Replace(job.JobName, postData.Text);
                job.JobName = postData.Text;
                vm.success = _dbContext.Update(job);
            }
            else if (postData.Operation == "move_node")
            {
                var job = _dbContext.Get<OrgJob>(postData.Id);
                job.Pid = postData.Parent;
                var parentJob = _dbContext.Get<OrgJob>(postData.Parent);
                job.FullName = parentJob == null ? postData.Text : $"{parentJob.FullName}/{postData.Text}";
                job.TreeLevel = parentJob == null ? 1 : parentJob.TreeLevel + 1;
                job.IsFinal = parentJob == null ? 0 : job.IsFinal;
                if (parentJob != null)
                {
                    parentJob.IsFinal = 0;
                    _dbContext.Update(parentJob);
                }
                vm.success = _dbContext.Update(job);
            }
            else if (postData.Operation == TreeNodeOper.COPY_NODE)
            {
                throw new NotImplementedException();
            }
            return vm;
        }
    }
}
