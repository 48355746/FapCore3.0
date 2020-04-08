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
            var payCase = _dbContext.Get<PayCase>(caseUid);
            if (payCase.Unchanged == 1)
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
            var payCase = _dbContext.Get<PayCase>(caseUid);
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
                col.Fid = null;
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
        [Transactional]
        public void InitEmployeeToPayCase(PayCase payCase, string empWhere)
        {
            if (payCase.Unchanged == 1)
            {
                throw new FapException("已存在薪资方法记录，不能再初始化员工");
            }
            if (!empWhere.Contains("IsMainJob", StringComparison.OrdinalIgnoreCase))
            {
                empWhere = empWhere.IsMissing() ? " IsMainJob=1" : empWhere + " and IsMainJob=1";
            }
            var employees = _dbContext.QueryWhere<Employee>(empWhere);
            IList<FapDynamicObject> listCase = new List<FapDynamicObject>();
            foreach (var emp in employees)
            {
                FapDynamicObject fdo = new FapDynamicObject(_dbContext.Columns(payCase.TableName));
                fdo.SetValue("EmpUid", emp.Fid);
                fdo.SetValue("EmpCode", emp.EmpCode);
                fdo.SetValue("EmpCategory", emp.EmpCategory);
                fdo.SetValue("DeptUid", emp.DeptUid);
                fdo.SetValue("PayCaseUid", payCase.Fid);
                fdo.SetValue("PayYM", payCase.PayYM.IsPresent() ? payCase.PayYM : payCase.InitYM);
                fdo.SetValue("PaymentTimes", 1);
                fdo.SetValue("PayConfirm", 0);
                listCase.Add(fdo);
            }
            _dbContext.ExecuteOriginal($"truncate table {payCase.TableName}");
            _dbContext.InsertDynamicDataBatchSql(listCase);
            //添加发放记录
            var records = _dbContext.DeleteExec(nameof(PayRecord), "CaseUid=@CaseUid"
                , new DynamicParameters(new { CaseUid = payCase.Fid }));
            PayRecord pr = new PayRecord();
            pr.CaseUid = payCase.Fid;
            pr.PayCount = 1;
            pr.PayYM = payCase.InitYM;
            pr.PayFlag = 0;
            _dbContext.Insert<PayRecord>(pr);
            //更新薪资套本次发放内容
            payCase.PayYM = payCase.InitYM;
            payCase.PayCount = 1;
            payCase.PayFlag = 0;
            _dbContext.Update(payCase);
        }
        [Transactional]
        public void InitPayrollData(PayrollInitDataViewModel payrollInitData)
        {
            PayCase payCase = _dbContext.Get<PayCase>(payrollInitData.CaseUid);
            PayRecord pRecord = _dbContext.Get<PayRecord>(payrollInitData.PayRecordUid);

            string where = " PayYM='" + pRecord.PayYM + "' and PayCaseUid='" + pRecord.CaseUid + "' and PaymentTimes=" + pRecord.PayCount;
            var colList = _dbContext.Columns(payCase.TableName);
            //基础列和 特性列
            List<FapColumn> baseCols = colList.Where(c => (c.IsDefaultCol == 1 || c.ColProperty == "3") && c.ColName != "Id"&&c.ColName!="Fid").ToList();
            string pCols = string.Join(",", baseCols.Select(c => c.ColName));
            if (payrollInitData.ReservedItems.IsPresent())
            {
                pCols += "," + payrollInitData.ReservedItems;
            }
            pCols = pCols.ReplaceIgnoreCase("PayYM", "'" + payrollInitData.PayYm + "'");
            
            //检查当月是否有发送记录
            DynamicParameters param = new DynamicParameters();
            param.Add("PayYM", payrollInitData.PayYm);
            param.Add("CaseUid", payrollInitData.CaseUid);
            var records = _dbContext.QueryWhere<PayRecord>(" PayYM=@PayYM and CaseUid=@CaseUid", param);
            int pcount = 1;
            if (records.Any())
            {
                pcount = records.Max(r => r.PayCount) + 1;
            }
            pCols = pCols.ReplaceIgnoreCase("PaymentTimes", pcount.ToString());
            pCols = pCols.ReplaceIgnoreCase("PayConfirm", "0");               
            string sql = $"select {pCols} from PayCenter where {where}";
            var pastData = _dbContext.QueryOriSql(sql);
            IList<FapDynamicObject> listCase = new List<FapDynamicObject>();
            foreach (var pd in pastData)
            {
                var dicPd = pd as IDictionary<string, object>;
                FapDynamicObject fdo = new FapDynamicObject(_dbContext.Columns(payCase.TableName));
                foreach (var key in dicPd.Keys)
                {
                    fdo.SetValue(key, dicPd[key]);
                }
                listCase.Add(fdo);
            }
            _dbContext.ExecuteOriginal($"truncate table {payCase.TableName}");
            _dbContext.InsertDynamicDataBatchSql(listCase);
            //添加发放记录
            PayRecord newRecord = new PayRecord();
            newRecord.CaseUid = payCase.Fid;
            newRecord.PayCount = pcount;
            newRecord.PayYM = payrollInitData.PayYm;
            newRecord.PayFlag = 0;
            _dbContext.Insert(newRecord);
            //更新工资套
            payCase.PayYM = payrollInitData.PayYm;
            payCase.PayCount = pcount;
            payCase.PayFlag = 0;
            _dbContext.Update(payCase);
            
        }
        /// <summary>
        /// 应用待处理
        /// </summary>
        /// <param name="payToDo"></param>
        [Transactional]
        public void UsePayPending(PayToDo payToDo)
        {
            //薪资套
            PayCase payCase = _dbContext.Get<PayCase>(payToDo.CaseUid);
            //员工
            Employee employee = _dbContext.Get<Employee>(payToDo.EmpUid);
            //检查员工是否在薪资套
            var caseEmployee = _dbContext.QueryFirstOrDefault($"select * from {payCase.TableName} where EmpUid=@EmpUid", new DynamicParameters(new { EmpUid = employee.Fid }));
            if (caseEmployee != null)
            {
                if (employee.EmpStatus == EmployeeStatus.Former)
                {
                    DeleteEmployeeFromPayCase();
                }
                else
                {
                    UpdateEmployeeToPayCase();
                }
            }
            else if (employee.EmpStatus == EmployeeStatus.Current)
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
                caseEmp.SetValue("PayCaseUid", payCase.Fid);
                caseEmp.SetValue("PaymentTimes", 1);
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
