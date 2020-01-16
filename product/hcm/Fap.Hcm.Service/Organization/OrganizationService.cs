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

namespace Fap.Hcm.Service.Organization
{
    [Service]
    public class OrganizationService : IOrganizationService
    {
        private readonly IDbContext _dbContext;
        private readonly IFapPlatformDomain _platformDomain;
        public OrganizationService(IDbContext dbContext,IFapPlatformDomain platformDomain)
        {
            _dbContext = dbContext;
            _platformDomain = platformDomain;
        }
        [Transactional]
        public ResponseViewModel MoveDepartment(TreePostData postData)
        {
            Guard.Against.Null(postData,nameof(postData));
            if (postData.Operation == TreeNodeOper.MOVE_NODE)
            {
                //当前部门
                _platformDomain.OrgDeptSet.TryGetValue(postData.Id, out OrgDept currDept);

                //检查之前父部门子级
                _platformDomain.OrgDeptSet.TryGetValueByPid(currDept.Pid, out IEnumerable<OrgDept> childs);
                if (childs.Any() && childs.Count() == 1)
                {
                    if(_platformDomain.OrgDeptSet.TryGetValue(currDept.Pid, out OrgDept parentDept))
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

        public ResponseViewModel MergeDepartment(MergeDeptModel mergeDept)
        {
            Guard.Against.Null(mergeDept, nameof(mergeDept));

            var employees = _dbContext.QueryWhere<Employee>("DeptUid in @Depts", new Dapper.DynamicParameters(new { Depts = mergeDept.MergeFids }));
            foreach (var employee in employees)
            {
                employee.DeptUid = mergeDept.DeptFid;
                employee.DeptCode = mergeDept.DeptCode;
                _dbContext.Update(employee);
            }
            //删除旧部门
            var mergeDepts= _platformDomain.OrgDeptSet.Where(d => mergeDept.MergeFids.Contains(d.Fid));
            foreach (var dept in mergeDepts)
            {
                _dbContext.Delete(dept);
            }
            return ResponseViewModelUtils.Sueecss();
        }
    }
}
