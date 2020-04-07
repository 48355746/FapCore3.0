using Fap.Core.DataAccess;
using Fap.Core.DI;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.Infrastructure.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fap.Core.Extensions;
using Fap.Core.Exceptions;
using Dapper;
using Ardalis.GuardClauses;
using Fap.Core.Utility;
using Fap.Core.Rbac.Model;

namespace Fap.Hcm.Service.Payroll
{
    [Service]
    public class PayrollService : IPayrollService
    {
        private const string PAYROLLCENTER = "PayCenter";
        private readonly IDbContext _dbContext;
       
        private readonly IFapApplicationContext _applicationContext;
        private readonly IFapPlatformDomain _platformDomain;
        public PayrollService(IDbContext dataAccessor, IFapPlatformDomain platformDomain, IFapApplicationContext applicationContext)
        {
            _dbContext = dataAccessor;
            _platformDomain = platformDomain;
            _applicationContext = applicationContext;
        }
        [Transactional]
        public void AddPayItem(string caseUid, string[] payItems)
        {
            if (payItems == null || payItems.Length < 1 || caseUid.IsMissing())
            {
                return;
            }
            DynamicParameters param = new DynamicParameters();
            param.Add("CaseUid", caseUid);
            int pcount = _dbContext.Count("PayRecord", "CaseUid=@CaseUid  and PayFlag=1", param);
            if (pcount > 0)
            {
                throw new FapException("已经存在发放记录，不能再调整薪资项");
            }
            var items = _dbContext.Columns(PAYROLLCENTER).Where(f => f.ColProperty.EqualsWithIgnoreCase("3") || payItems.Contains(f.Fid));
            IList<PayItem> list = new List<PayItem>();
            foreach (var item in items)
            {
                list.Add(new PayItem { CaseUid = caseUid, ColumnUid = item.Fid, ItemSort = item.ColOrder, ItemType = "Normal", ShowAble = 1, ShowCard = 0, AddUpWay = "No", TransEnable = 0 });
            }
            _dbContext.DeleteExec(nameof(PayItem), "CaseUid=@CaseUid", param);
            _dbContext.InsertBatchSql<PayItem>(list);
        }

        public IEnumerable<PayCaseItem> GetPayCaseItem(string caseUid)
        {
            PayCaseItem pi = new PayCaseItem();
            var payItems = _dbContext.Columns(PAYROLLCENTER).Where(c => c.IsDefaultCol != 1 && !c.ColProperty.Trim().EqualsWithIgnoreCase("3"))
                 .Select(c => new PayCaseItem { Fid = c.Fid, ColComment = c.ColComment, ColName = c.ColName, IsSelected = false }).ToList();
            var caseItems = _dbContext.QueryWhere<PayItem>("CaseUid=@CaseUid", new Dapper.DynamicParameters(new { CaseUid = caseUid }))
                .Select(c => c.ColumnUid);
            if (caseItems.Any())
            {
                foreach (var item in payItems)
                {
                    if (caseItems.Contains(item.Fid))
                    {
                        item.IsSelected = true;
                    }
                }
            }
            return payItems;
        }
        [Transactional]
        public long GenericPayCase(string caseUid)
        {
            var payCase= _dbContext.Get<PayCase>(caseUid);
            //生成工资套对应表元数据
            FapTable ft = new FapTable();
            ft.Fid = UUIDUtils.Fid;
            ft.TableName = $"PayCase{_applicationContext.TenantID}{payCase.CaseCode}";
            ft.TableType = "BUSINESS";
            ft.TableCategory = "Pay";
            ft.TableComment = $"{payCase.CaseName}薪资套";
            ft.TableMode = "SINGLE";
            ft.TableFeature = "";//根据ColProperty='3'过滤，不在用TableFeature
            ft.IsSync = 1;
            ft.IsBasic = 1;
            ft.ProductUid = "HCM";

            IEnumerable<FapColumn> centerCols = _dbContext.Columns(PAYROLLCENTER);
            var caseItems = _dbContext.QueryWhere<PayItem>("CaseUid=@CaseUid", new Dapper.DynamicParameters(new { CaseUid = caseUid }))
               .Select(c => c.ColumnUid);
            List<FapColumn> cols = new List<FapColumn>();
            foreach (var col in centerCols.Where(c => caseItems.Contains(c.Fid)))
            {
                col.Fid =null;
                col.TableName = ft.TableName;
                cols.Add(col);
            }
            DynamicParameters param = new DynamicParameters();
            param.Add("TableName", ft.TableName);
            _dbContext.Execute("delete from FapTable where TableName=@TableName", param);
            _dbContext.Execute("delete from FapColumn where TableName=@TableName", param);
            _dbContext.Execute($"delete from FapMultiLanguage where LangKey like '{ft.TableName}_%'");
            _dbContext.Insert<FapTable>(ft);
            _dbContext.InsertBatch<FapColumn>(cols);
            _platformDomain.TableSet.Refresh();
            _platformDomain.ColumnSet.Refresh();
            _platformDomain.MultiLangSet.Refresh();
            payCase.TableName = ft.TableName;
            _dbContext.Update(payCase);
            return ft.Id;
        }
        /// <summary>
        /// 应用待处理
        /// </summary>
        /// <param name="payToDo"></param>
        [Transactional]
        public void UsePayPending(PayToDo payToDo)
        {
            //薪资套
            PayCase payCase= _dbContext.Get<PayCase>(payToDo.CaseUid);
            //员工
            Employee employee = _dbContext.Get<Employee>(payToDo.EmpUid);
            //检查员工是否在薪资套
            var caseEmployee = _dbContext.QueryFirstOrDefault($"select * from {payCase.TableName} where EmpUid=@EmpUid", new DynamicParameters(new { EmpUid=employee.Fid}));
            if (caseEmployee!=null)
            {
                if (employee.EmpStatus == EmployeeStatus.Former)
                {
                    DeleteEmployeeFromPayCase();
                }
                else
                {
                    UpdateEmployeeToPayCase();
                }
            }else if (employee.EmpStatus == EmployeeStatus.Current)
            {
                AddEmployeeToPayCase();
            }
            MarkPayTodo();
            void MarkPayTodo()
            {
                payToDo.OperDate = DateTimeUtils.CurrentDateTimeStr;
                payToDo.OperEmpUid = _applicationContext.EmpUid;
                payToDo.OperFlag = "1";
                _dbContext.Update(payToDo);
            }
            void DeleteEmployeeFromPayCase()
            {
                string sql = $"delete from {payCase.TableName} where Fid=@Fid";
                _dbContext.ExecuteOriginal(sql, new DynamicParameters(new { Fid = caseEmployee.Fid }));
            }
            void UpdateEmployeeToPayCase()
            {
                string sql = $"update {payCase.TableName} set EmpCode='{employee.EmpCode}',EmpCategory='{employee.EmpCategory}',DeptUid='{employee.DeptUid}' where Fid=@Fid";
                _dbContext.ExecuteOriginal(sql, new DynamicParameters(new { Fid = caseEmployee.Fid }));
            }
            void AddEmployeeToPayCase()
            {
                FapDynamicObject caseEmp = new FapDynamicObject(_dbContext.Columns(payCase.TableName));
                //将此人的放入薪资套
                caseEmp.SetValue("EmpUid", employee.Fid);
                caseEmp.SetValue("EmpCode", employee.EmpCode);
                caseEmp.SetValue("DeptUid", employee.DeptUid);
                caseEmp.SetValue("PayCaseUid",payCase.Fid);
                caseEmp.SetValue("PaymentTimes",1);
                caseEmp.SetValue("PayConfirm", 0);

                if (payCase.PayYM.IsPresent())
                {
                    caseEmp.SetValue("PayYM", payCase.PayYM);
                }
                else
                {
                    caseEmp.SetValue("PayYM", payCase.InitYM);
                }
                _dbContext.InsertDynamicData(caseEmp);
            }

        }
    }
}
