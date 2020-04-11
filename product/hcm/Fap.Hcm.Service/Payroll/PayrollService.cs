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
using Fap.Core.Infrastructure.Model;

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
                list.Add(new PayItem { CaseUid = caseUid, ColumnUid = item.Fid, ItemSort = item.ColOrder, ShowAble = 1, ShowCard = 0, TransEnable = 0 });
            }
            _dbContext.DeleteExec(nameof(PayItem), "CaseUid=@CaseUid", param);
            _dbContext.InsertBatchSql<PayItem>(list);
        }
        /// <summary>
        /// 薪资项
        /// </summary>
        /// <param name="caseUid"></param>
        /// <returns></returns>
        public IEnumerable<CaseItem> GetPayCaseItem(string caseUid)
        {
            CaseItem pi = new CaseItem();
            var payItems = _dbContext.Columns(PAYROLLCENTER).Where(c => c.IsDefaultCol != 1 && !c.ColProperty.Trim().EqualsWithIgnoreCase("3"))
                 .Select(c => new CaseItem { Fid = c.Fid, ColComment = c.ColComment, ColName = c.ColName, IsSelected = false }).ToList();
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
        public long CreatePayCase(string caseUid)
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
        public void InitPayrollData(InitDataViewModel payrollInitData)
        {
            PayCase payCase = _dbContext.Get<PayCase>(payrollInitData.CaseUid);
            PayRecord pRecord = _dbContext.Get<PayRecord>(payrollInitData.RecordUid);

            string where = " PayYM='" + pRecord.PayYM + "' and PayCaseUid='" + pRecord.CaseUid + "' and PaymentTimes=" + pRecord.PayCount;
            var colList = _dbContext.Columns(payCase.TableName);
            //基础列和 特性列
            List<FapColumn> baseCols = colList.Where(c => (c.IsDefaultCol == 1 || c.ColProperty == "3") && c.ColName != "Id" && c.ColName != "Fid").ToList();
            string pCols = string.Join(",", baseCols.Select(c => c.ColName));
            if (payrollInitData.ReservedItems.IsPresent())
            {
                pCols += "," + payrollInitData.ReservedItems;
            }
            pCols = pCols.ReplaceIgnoreCase("PayYM", "'" + payrollInitData.InitYm + "' as PayYM");

            //检查当月是否有发送记录
            DynamicParameters param = new DynamicParameters();
            param.Add("PayYM", payrollInitData.InitYm);
            param.Add("CaseUid", payrollInitData.CaseUid);
            var records = _dbContext.QueryWhere<PayRecord>(" PayYM=@PayYM and CaseUid=@CaseUid", param);
            int pcount = 1;
            if (records.Any())
            {
                var existRecord= records.FirstOrDefault(r => r.PayFlag == 0);
                if (existRecord==null)
                {
                    pcount = records.Max(r => r.PayCount) + 1;
                    //添加发放记录
                    PayRecord newRecord = new PayRecord();
                    newRecord.CaseUid = payCase.Fid;
                    newRecord.PayCount = pcount;
                    newRecord.PayYM = payrollInitData.InitYm;
                    newRecord.PayFlag = 0;
                    _dbContext.Insert(newRecord);
                }
                else
                {
                    pcount = existRecord.PayCount;
                }
            }
            else
            {
                //添加发放记录
                PayRecord newRecord = new PayRecord();
                newRecord.CaseUid = payCase.Fid;
                newRecord.PayCount = pcount;
                newRecord.PayYM = payrollInitData.InitYm;
                newRecord.PayFlag = 0;
                _dbContext.Insert(newRecord);
            }
            pCols = pCols.ReplaceIgnoreCase("PaymentTimes", pcount.ToString()+ " as PaymentTimes");
            pCols = pCols.ReplaceIgnoreCase("PayConfirm", "0 as PayConfirm");
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

            //更新工资套
            payCase.PayYM = payrollInitData.InitYm;
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
                caseEmp.SetValue("EmpCategory", employee.EmpCategory);
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

        [Transactional]
        public void PayrollOff(string caseUid)
        {
            PayCase payCase = _dbContext.Get<PayCase>(caseUid);
            if (payCase.PayFlag == 1)
            {
                throw new FapException("薪资已经发放，请不要重复发放");
            }
            payCase.PayFlag = 1;
            //标记已存在发放，不能再设置薪资套
            payCase.Unchanged = 1;
            _dbContext.Update(payCase);
            //获取发放记录
            DynamicParameters paramR = new DynamicParameters();
            paramR.Add("PayYM", payCase.PayYM);
            paramR.Add("PayCount", payCase.PayCount);
            paramR.Add("CaseUid", payCase.Fid);
            PayRecord payRecord = _dbContext.QueryFirstOrDefaultWhere<PayRecord>("PayYM=@PayYM and PayCount=@PayCount and CaseUid=@CaseUid and PayFlag=0", paramR);
            if (payRecord != null)
            {
                payRecord.PayFlag = 1;
                payRecord.PayEmpUid = _applicationContext.EmpUid;
                payRecord.PayDate = DateTimeUtils.CurrentDateTimeStr;
                _dbContext.Update(payRecord);
            }

            var caseCols = _dbContext.Columns(payCase.TableName)
                .Where(c => !c.ColName.EqualsWithIgnoreCase("Id"))
                .Select(c => c.ColName);
            string cols = string.Join(",", caseCols);
            string insertSql = string.Format("insert into {0}({1}) (select {1} from {2})", "PayCenter", cols, payCase.TableName);
            _dbContext.ExecuteOriginal(insertSql);
            string updateSql = string.Format("update {0} set PayConfirm=1", payCase.TableName);
            _dbContext.ExecuteOriginal(updateSql);
        }
        /// <summary>
        /// 取消发放
        /// </summary>
        /// <param name="caseUid"></param>
        /// <returns></returns>
        public void PayrollOffCancel(string caseUid)
        {
            PayCase pc = _dbContext.Get<PayCase>(caseUid);
            if (pc.PayFlag == 0)
            {
                throw new FapException("薪资未发放，不用取消");
            }
            pc.PayFlag = 0;
            if (pc.InitYM == pc.PayYM && pc.PayCount == 1)
            {
                pc.Unchanged = 0;
            }
            _dbContext.Update(pc);
            //获取发放记录
            DynamicParameters paramR = new DynamicParameters();
            paramR.Add("PayYM", pc.PayYM);
            paramR.Add("PayCount", pc.PayCount);
            paramR.Add("CaseUid", pc.Fid);
            PayRecord pr = _dbContext.QueryFirstOrDefaultWhere<PayRecord>("PayYM=@PayYM and PayCount=@PayCount and CaseUid=@CaseUid and PayFlag=1", paramR);
            if (pr != null)
            {
                pr.PayFlag = 0;
                pr.PayDate = "";
                pr.PayEmpUid = "";
                _dbContext.Update(pr);
            }
            string deleteSql = "delete from PayCenter where PayCaseUid=@CaseUid and PaymentTimes=@PayTimes and PayYM=@PayYM";
            string updateSql = string.Format("update {0} set PayConfirm=0", pc.TableName);
            _dbContext.Execute(deleteSql, new DynamicParameters(new { CaseUid = pc.Fid, PayTimes = pc.PayCount, PayYM = pc.PayYM }));
            _dbContext.Execute(updateSql);
        }
        public GapEmployee PayGapAnalysis(string recordUid)
        {
            PayRecord payRecord= _dbContext.Get<PayRecord>(recordUid);
            PayCase payCase = _dbContext.Get<PayCase>(payRecord.CaseUid);
            //当前月有 历史数据没有的语句（入职的）
            string sql1 = string.Format($"select EmpUid from {payCase.TableName} where PaymentTimes={payCase.PayCount} and EmpUid NOT IN(select EmpUid from {PAYROLLCENTER} where  PayYM='{payRecord.PayYM}' and PayCaseUid='{payRecord.CaseUid}' and PaymentTimes={payRecord.PayCount})");
            //当前月没有 历史数据有的（离职的）
            string sql2 = string.Format($"select EmpUid from {PAYROLLCENTER} where  PayYM='{payRecord.PayYM}' and PayCaseUid='{payRecord.CaseUid}' and PaymentTimes={payRecord.PayCount} and EmpUid NOT IN(select EmpUid from {payCase.TableName} where PaymentTimes={payCase.PayCount})");
            var list1 = _dbContext.QueryOriSql(sql1);
            var list2 = _dbContext.QueryOriSql(sql2);
            GapEmployee emps = new GapEmployee();
            if (list1.Any())
            {
                emps.AddedList = _dbContext.QueryWhere<Employee>("Fid in @Fids",new DynamicParameters(new { Fids=list1.Select(l=>l.EmpUid) }));
            }
            if (list2.Any())
            {
                emps.RemovedList = _dbContext.QueryWhere<Employee>("Fid in @Fids", new DynamicParameters(new { Fids = list2.Select(l => l.EmpUid) }));
            }
            return emps;
        }
        public void PayrollOffNotice(string caseUid)
        {
            PayCase pc = _dbContext.Get<PayCase>(caseUid);
            if (pc.PayFlag == 0)
            {
                throw new FapException("薪资还未发放，还不能发送通知");
            }
            string sql = $"select {pc.TableName}.EmpUid,Employee.Mailbox,Employee.EmpName from {pc.TableName} left join Employee on {pc.TableName}.EmpUid=Employee.Fid";
            var empList = _dbContext.Query(sql);
            if (empList.Any())
            {
                string mailContent = pc.PayYM + "份薪资已发放，请登录HCM系统查看。员工自助--》我的薪资";
                string msgContent = pc.PayYM + "份薪资已发放。";
                string sendTime = DateTimeUtils.CurrentDateTimeStr;             
                List<string> lmail = new List<string>();
                List<FapMessage> lmsg = new List<FapMessage>();
                foreach (var emp in empList)
                {
                    FapMessage message = new FapMessage { HasRead = 0, MsgCategory = "Notice", MsgContent = msgContent, REmpUid = emp.EmpUid, SendTime = sendTime, Title = "薪资发放通知" };
                    lmsg.Add(message);
                    string mailBox = emp.Mailbox;
                    if (mailBox.IsPresent())
                    {
                        lmail.Add($"{emp.EmpName}<{emp.Mailbox}>");
                    }
                }
                _dbContext.InsertBatchSql<FapMessage>(lmsg);
                if (lmail.Any())
                {
                    FapMail fmail = new FapMail();
                    fmail.IsSeparate = 1;
                    fmail.Recipient = "薪资套人员";
                    fmail.Subject = "薪资发放通知";
                    fmail.MailContent = mailContent;
                    fmail.RecipientEmailAddress = string.Join(";", lmail);
                    fmail.SendStatus = 0;
                    _dbContext.Insert<FapMail>(fmail);
                }

            }
        }
    }
}
