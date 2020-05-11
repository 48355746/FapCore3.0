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
using Fap.Core.Infrastructure.Config;
using Fap.Core.Utility;

namespace Fap.Hcm.Service.Organization
{
    [Service]
    public class OrganizationService : IOrganizationService
    {
        private readonly IDbContext _dbContext;
        private readonly IFapPlatformDomain _platformDomain;
        private readonly IFapConfigService _configService;
        private readonly IFapApplicationContext _applicationContext;
        public OrganizationService(IDbContext dbContext,
            IFapPlatformDomain platformDomain,
            IFapConfigService configService,
            IFapApplicationContext applicationContext)
        {
            _dbContext = dbContext;
            _platformDomain = platformDomain;
            _configService = configService;
            _applicationContext = applicationContext;
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

        public List<TreeDataView> GetOrgJobGroupTree()
        {
            IEnumerable<OrgJobGroup> jobGroups = _dbContext.QueryAll<OrgJobGroup>();
            List<TreeDataView> oriGroupList = jobGroups.Select(t => new TreeDataView { Id = t.Fid, Pid = "0", Data = new { group = "1" }, Text = t.Name, Icon = "icon-folder orange ace-icon fa fa-users" }).ToList<TreeDataView>();
            IEnumerable<OrgJobGroupGrade> jobGroupGrades = _dbContext.QueryAll<OrgJobGroupGrade>();
            List<TreeDataView> oriGroupGradeList = jobGroupGrades.Select(t => new TreeDataView { Id = t.Fid, Pid = t.JobGroup, Data = new { group = "0" }, Text = t.Name, Icon = "icon-folder orange ace-icon fa fa-users" }).ToList<TreeDataView>();
            List<TreeDataView> tree = new List<TreeDataView>();
            TreeDataView treeRoot = new TreeDataView()
            {
                Id = "0",
                Text = "职族",
                Data = new { group = "3" },
                State = new NodeState { Opened = true },
                Icon = "icon-folder blue ace-icon fa fa-sitemap",
            };
            tree.Add(treeRoot);
            TreeViewHelper.MakeTree(treeRoot.Children, oriGroupList.Union(oriGroupGradeList), treeRoot.Id);
            return tree;
        }
        /// <summary>
        /// 获取管辖的部门
        /// </summary>
        /// <returns></returns>
        public IEnumerable<OrgDept> GetDominationDepartment()
        {
            var param = new Dapper.DynamicParameters(new { _applicationContext.EmpUid });
            var deptList = _dbContext.QueryWhere<OrgDept>("DeptManager=@EmpUid", param);
            string director = _configService.GetSysParamValue("org.permissions.director");
            if (director.ToBool())
            {
                deptList = deptList.Concat(_dbContext.QueryWhere<OrgDept>("Director=@EmpUid", param));
            }
            IEnumerable<OrgDept> dominations = Enumerable.Empty<OrgDept>();
            foreach (var dept in deptList)
            {
                dominations=dominations.Concat(_platformDomain.OrgDeptSet.Where(d => d.DeptCode.StartsWith(dept.DeptCode, StringComparison.OrdinalIgnoreCase)));
            }
            return dominations.Distinct(new FapModelEqualityComparer<OrgDept>());
        }
        public IEnumerable<TreeDataView> GetDominationDepartmentTree()
        {
            List<TreeDataView> tree = new List<TreeDataView>();
            TreeDataView treeRoot = new TreeDataView()
            {
                Id = "0",
                Text = "管辖部门",
                Data = new { isDept = 0 },
                State = new NodeState { Opened = true },
                Icon = "icon-folder blue ace-icon fa fa-sitemap",
            };
            tree.Add(treeRoot);
            var param = new Dapper.DynamicParameters(new { _applicationContext.EmpUid });
            var deptList = _dbContext.QueryWhere<OrgDept>("DeptManager=@EmpUid", param);
            string director = _configService.GetSysParamValue("org.permissions.director");
            if (director.ToBool())
            {
                deptList = deptList.Concat(_dbContext.QueryWhere<OrgDept>("Director=@EmpUid", param));
            }
            foreach (var dept in deptList.Distinct(new FapModelEqualityComparer<OrgDept>()))
            {
                 var childs= _platformDomain.OrgDeptSet.Where(d => d.DeptCode.StartsWith(dept.DeptCode, StringComparison.OrdinalIgnoreCase))
                    .Select(t=>new TreeDataView {Id=t.Fid,Pid=(t.Fid==dept.Fid?"0":t.Pid),Text=t.DeptName,Data=new { isDept=1},State= new NodeState { Opened = true }, Icon = "icon-folder blue ace-icon fa fa-folder-o " }) ;
                TreeViewHelper.MakeTree(treeRoot.Children, childs, treeRoot.Id);
            }
            return tree;
        }

    }
}
