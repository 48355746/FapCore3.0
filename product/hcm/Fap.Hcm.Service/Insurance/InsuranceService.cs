using Dapper;
using Fap.Core.DataAccess;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.Infrastructure.Metadata;
using Fap.Core.Rbac.Model;
using Fap.Core.Extensions;
using Fap.Hcm.Service.Payroll;
using System;
using System.Collections.Generic;
using System.Text;
using Fap.Core.Utility;

namespace Fap.Hcm.Service.Insurance
{
    public class InsuranceService : IInsuranceService
    {
        private readonly IDbContext _dbContext;

        private readonly IFapApplicationContext _applicationContext;
        private readonly IFapPlatformDomain _platformDomain;
        public InsuranceService(IDbContext dataAccessor, IFapPlatformDomain platformDomain, IFapApplicationContext applicationContext)
        {
            _dbContext = dataAccessor;
            _platformDomain = platformDomain;
            _applicationContext = applicationContext;
        }
        public void UseInsPending(InsToDo insToDo)
        {
            //保险组
            InsCase insCase = _dbContext.Get<InsCase>(insToDo.CaseUid);
            //员工
            Employee employee = _dbContext.Get<Employee>(insToDo.EmpUid);
            //检查员工是否在保险组
            var caseEmployee = _dbContext.QueryFirstOrDefault($"select * from {insCase.TableName} where EmpUid=@EmpUid", new DynamicParameters(new { EmpUid = employee.Fid }));
            if (caseEmployee != null)
            {
                if (employee.EmpStatus == EmployeeStatus.Former)
                {
                    DeleteEmployeeFromInsCase();
                }
                else
                {
                    UpdateEmployeeToInsCase();
                }
            }
            else if (employee.EmpStatus == EmployeeStatus.Current)
            {
                AddEmployeeToInsCase();
            }
            MarkInsTodo();
            void MarkInsTodo()
            {
                insToDo.OperDate = DateTimeUtils.CurrentDateTimeStr;
                insToDo.OperEmpUid = _applicationContext.EmpUid;
                insToDo.OperFlag = "1";
                _dbContext.Update(insToDo);
            }
            void DeleteEmployeeFromInsCase()
            {
                string sql = $"delete from {insCase.TableName} where Fid=@Fid";
                _dbContext.ExecuteOriginal(sql, new DynamicParameters(new { Fid = caseEmployee.Fid }));
            }
            void UpdateEmployeeToInsCase()
            {
                string sql = $"update {insCase.TableName} set EmpCode='{employee.EmpCode}',EmpCategory='{employee.EmpCategory}',DeptUid='{employee.DeptUid}' where Fid=@Fid";
                _dbContext.ExecuteOriginal(sql, new DynamicParameters(new { Fid = caseEmployee.Fid }));
            }
            void AddEmployeeToInsCase()
            {
                FapDynamicObject caseEmp = new FapDynamicObject(_dbContext.Columns(insCase.TableName));
                //将此人的放入薪资套
                caseEmp.SetValue("EmpUid", employee.Fid);
                caseEmp.SetValue("EmpCode", employee.EmpCode);
                caseEmp.SetValue("EmpCategory", employee.EmpCategory);
                caseEmp.SetValue("DeptUid", employee.DeptUid);
                caseEmp.SetValue("InsCaseUid", insCase.Fid);
                caseEmp.SetValue("InsmentTimes", 1);
                caseEmp.SetValue("InsConfirm", 0);

                if (insCase.InsYM.IsPresent())
                {
                    caseEmp.SetValue("InsYM", insCase.InsYM);
                }
                else
                {
                    caseEmp.SetValue("InsYM", insCase.InitYM);
                }
                _dbContext.InsertDynamicData(caseEmp);
            }
        }
    }
}
