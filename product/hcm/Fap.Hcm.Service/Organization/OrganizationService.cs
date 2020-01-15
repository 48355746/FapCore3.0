using Fap.AspNetCore.ViewModel;
using Fap.Core.DataAccess;
using Fap.Core.DI;
using Fap.Core.Rbac.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fap.Hcm.Service.Organization
{
    [Service]
    public class OrganizationService : IOrganizationService
    {
        private readonly IDbContext _dbContext;
        public OrganizationService(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public ResponseViewModel MoveDepartment(TreePostData postData)
        {
            bool success = false;
            if (postData.Operation == "move_node")
            {
                //父级部门
                OrgDept pOrgDept = _dbContext.Get<OrgDept>(parent);
                OrgDept currDept = _dbContext.Get<OrgDept>(id);
                DynamicParameters param = new DynamicParameters();
                param.Add("Pid", parent);
                var maxCodeStr = _dbContext.ExecuteScalar("select max(DeptCode) from OrgDept where  Pid=@Pid", param);
                int maxLength = 0;
                int maxOrder = 1;
                string deptCode = "";
                if (maxCodeStr != null)
                {
                    maxLength = maxCodeStr.ToString().Length;
                    maxOrder = maxCodeStr.ToString().Substring(maxLength - 2).ToInt() + 1;
                    int maxCode = maxCodeStr.ToString().ToInt() + 1;
                    deptCode = maxCode.ToString().PadLeft(maxLength, '0');
                }
                else
                {
                    deptCode = pOrgDept.DeptCode + "01";
                }
                dynamic fdo = new FapDynamicObject(_dbContext.Columns("OrgDept"));
                //fdo.TableName = "OrgDept";
                fdo.Pid = parent;
                fdo.Fid = id;//根据Fid进行更新操作
                fdo.DeptCode = deptCode;
                fdo.DeptOrder = currDept.DeptOrder;
                fdo.FullName = currDept.FullName;
                fdo.DeptName = currDept.DeptName;
                fdo.TreeLevel = currDept.TreeLevel;
                fdo.PCode = currDept.PCode;
                _dbContext.UpdateDynamicData(fdo);
                success = true;
                //DataAccessor.Excute("update FapUserGroup set Pid=@Pid where Id=@Id", new  { Pid=parent,Id=id});
            }
        }
    }
}
