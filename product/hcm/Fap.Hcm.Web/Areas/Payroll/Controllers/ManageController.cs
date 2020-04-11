using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Fap.AspNetCore.Infrastructure;
using Fap.AspNetCore.ViewModel;
using Fap.Core.Infrastructure.Metadata;
using Fap.Core.Infrastructure.Query;
using Fap.Hcm.Service.Payroll;
using Microsoft.AspNetCore.Mvc;

namespace Fap.Hcm.Web.Areas.Payroll.Controllers
{
    [Area("Payroll")]
    public class ManageController : FapController
    {
        public ManageController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
        /// <summary>
        /// 薪资待处理变动
        /// </summary>
        /// <returns></returns>
        public IActionResult Pending()
        {
            var jqModel = GetJqGridModel("PayToDo", (qs) =>
            {
                qs.GlobalWhere = "CaseUid in(select fid from PayCase where CreateBy=@EmpUid or fid in(select CaseUid from PayCaseEmployee where EmpUid=@EmpUid))";
                qs.AddParameter("EmpUid", _applicationContext.EmpUid);
                qs.InitWhere = "OperFlag=0";
            });        
            return View(jqModel);
        }
        /// <summary>
        /// 薪资套
        /// </summary>
        /// <returns></returns>
        public IActionResult PaySet()
        {
            JqGridViewModel model = this.GetJqGridModel("PayCase", qs =>
            {
                qs.GlobalWhere = "CreateBy=@EmpUid";
                qs.AddParameter("EmpUid", _applicationContext.EmpUid);
            });
            return View(model);
        }
        /// <summary>
        /// 薪资项
        /// </summary>
        /// <param name="caseUid"></param>
        /// <returns></returns>
        public PartialViewResult PayItem(string caseUid)
        {
            JqGridViewModel payItemModel = this.GetJqGridModel("PayItem", (q) =>
            {
                q.GlobalWhere = "CaseUid=@CaseUid";
                q.AddParameter("CaseUid", caseUid);
            });
            return PartialView(payItemModel);
        }

        /// <summary>
        /// 薪资员工
        /// </summary>
        /// <param name="caseUid"></param>
        /// <returns></returns>
        public PartialViewResult PayEmployee()
        {
            JqGridViewModel model = this.GetJqGridModel("Employee");
            return PartialView(model);
        }
        /// <summary>
        /// 薪资套权限
        /// </summary>
        /// <param name="caseUid"></param>
        /// <returns></returns>
        public PartialViewResult PayAuthority(string caseUid, string caseName)
        {
            JqGridViewModel model = this.GetJqGridModel("PayCaseEmployee", (q) =>
            {
                q.AddDefaultValue("CaseUid", caseUid);
                q.AddDefaultValue("CaseUidMC", caseName);
                q.GlobalWhere = "CaseUid=@CaseUid";
                q.AddParameter("CaseUid", caseUid);
            });
            return PartialView(model);
        }
        public IActionResult PayCalculate()
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("EmpUid", _applicationContext.EmpUid);
            IEnumerable<PayCase> payCases = _dbContext.QueryWhere<PayCase>("TableName!='' and CreateBy=@EmpUid or  Fid in(select CaseUid from PayCaseEmployee where EmpUid=@EmpUid)", param);
            return View(payCases);
        }
        public PartialViewResult PayInfo(string fid)
        {
            var pc = _dbContext.Get<PayCase>(fid);
            var statistics = _dbContext.Columns(pc.TableName).Where(c => c.CtrlType == FapColumn.CTRL_TYPE_MONEY);
            var model = GetJqGridModel(pc.TableName, qs =>
            {
                qs.AddDefaultValue("PayCaseUid", pc.Fid);
                qs.AddDefaultValue("PayCaseUidMC", pc.CaseName);
                qs.AddStatSet(StatSymbolEnum.Description, "'合计:' as PayYM");
                foreach (var statistic in statistics)
                {
                    qs.AddStatSet(StatSymbolEnum.SUM, statistic.ColName);
                }
            });
            return PartialView(model);
        }
        public IActionResult PayDataInit(string fid)
        {
            PayCase pc = _dbContext.Get<PayCase>(fid);
            IEnumerable<FapColumn> cList = _dbContext.Columns(pc.TableName);
            ViewBag.GridId = $"grid-{pc.TableName}";
            DynamicParameters param = new DynamicParameters();
            param.Add("CaseUid", pc.Fid);
            IEnumerable<PayRecord> records = _dbContext.QueryWhere<PayRecord>("CaseUid=@CaseUid and PayFlag=1", param).OrderByDescending(c => c.PayYM).OrderByDescending(c => c.PayFlag);
            if (!records.Any())
            {
                return Content("无薪资发放记录，不用初始化");
            }
            ViewBag.Records = records;
            return PartialView(cList);
        }
        public IActionResult PayGapAnalysis(string fid)
        {
            var payRecords= _dbContext.QueryWhere<PayRecord>($"{nameof(PayRecord.CaseUid)}=@CaseUid and {nameof(PayRecord.PayFlag)}=1",
                new DynamicParameters(new { CaseUid = fid }));
            if (payRecords.Any())
            {
                return PartialView(payRecords);
            }
            return Content("还没发薪记录，不能比对");
        }
        public IActionResult PayCenter()
        {
            var payRecords = _dbContext.Query<PayRecord>($"select top 30 * from {nameof(PayRecord)} where {nameof(PayRecord.PayFlag)}=1",null,true);
            return View(payRecords);
        }
        public IActionResult PayCenterInfo(string fid)
        {
            var pr = _dbContext.Get<PayRecord>(fid);
            var pc = _dbContext.Get<PayCase>(pr.CaseUid);
            var cols= _dbContext.Columns(pc.TableName).Select(c=>c.ColName);
            var model = GetJqGridModel("PayCenter", qs =>
            {
                qs.QueryCols = string.Join(',', cols);
                qs.GlobalWhere = "PayCaseUid=@CaseUid";
                qs.InitWhere = "PayYM=@PayYM and  PaymentTimes=@Times";
                qs.AddParameter("PayYM", pr.PayYM);
                qs.AddParameter("CaseUid", pr.CaseUid);
                qs.AddParameter("Times", pr.PayCount.ToString());
            });
            return PartialView(model);
        }
    }
}