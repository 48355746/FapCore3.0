using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Fap.AspNetCore.Infrastructure;
using Fap.AspNetCore.ViewModel;
using Fap.Core.Infrastructure.Metadata;
using Fap.Core.Infrastructure.Model;
using Fap.Core.Infrastructure.Query;
using Fap.Hcm.Service.Insurance;
using Microsoft.AspNetCore.Mvc;

namespace Fap.Hcm.Web.Areas.Insurance.Controllers
{
    [Area("Insurance")]
    public class ManageController : FapController
    {
        public ManageController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        /// <summary>
        /// 保险待处理变动
        /// </summary>
        /// <returns></returns>
        public IActionResult Pending()
        {
            var jqModel = this.GetJqGridModel(nameof(InsToDo), (qs) =>
            {
                qs.GlobalWhere = "CaseUid in(select fid from InsCase where CreateBy=@Employee or fid in(select CaseUid from InsCaseEmployee where EmpUid=@Employee))";
                qs.AddParameter("Employee", _applicationContext.EmpUid);
                qs.InitWhere = "OperFlag=0";
            });
            return View(jqModel);
        }
        public IActionResult InsSet()
        {
            JqGridViewModel model = this.GetJqGridModel(nameof(InsCase), qs =>
            {
                qs.GlobalWhere = "CreateBy=@EmpUid";
                qs.AddParameter("EmpUid", _applicationContext.EmpUid);
            });
            return View(model);

        }
        /// <summary>
        /// 保险项
        /// </summary>
        /// <param name="caseUid"></param>
        /// <returns></returns>
        public PartialViewResult InsItem(string caseUid)
        {
            JqGridViewModel insItemModel = this.GetJqGridModel(nameof(InsItem), (q) =>
            {
                q.GlobalWhere = "CaseUid=@CaseUid";
                q.AddParameter("CaseUid", caseUid);
            });
            return PartialView(insItemModel);
        }
        /// <summary>
        /// 保险组员工
        /// </summary>
        /// <param name="caseUid"></param>
        /// <returns></returns>
        public PartialViewResult InsEmployee()
        {
            JqGridViewModel model = this.GetJqGridModel("Employee");
            return PartialView(model);
        }
        /// <summary>
        /// 保险组权限
        /// </summary>
        /// <param name="caseUid"></param>
        /// <returns></returns>
        public PartialViewResult InsAuthority(string caseUid, string caseName)
        {
            JqGridViewModel model = this.GetJqGridModel("InsCaseEmployee", (q) =>
            {
                q.AddDefaultValue("CaseUid", caseUid);
                q.AddDefaultValue("CaseUidMC", caseName);
                q.GlobalWhere = "CaseUid=@CaseUid";
                q.AddParameter("CaseUid", caseUid);
            });
            return PartialView(model);
        }
        public IActionResult InsCalculate()
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("EmpUid", _applicationContext.EmpUid);
            IEnumerable<InsCase> insCases = _dbContext.QueryWhere<InsCase>("TableName!='' and CreateBy=@EmpUid or  Fid in(select CaseUid from InsCaseEmployee where EmpUid=@EmpUid)", param);
            return View(insCases);
        }
        public PartialViewResult InsInfo(string fid)
        {
            var ic = _dbContext.Get<InsCase>(fid);
            var statistics = _dbContext.Columns(ic.TableName).Where(c => c.CtrlType == FapColumn.CTRL_TYPE_MONEY);
            var model = GetJqGridModel(ic.TableName, qs =>
            {
                qs.AddDefaultValue("InsCaseUid", ic.Fid);
                qs.AddDefaultValue("InsCaseUidMC", ic.CaseName);
                qs.AddStatSet(StatSymbolEnum.Description, "'合计:' as InsYM");
                foreach (var statistic in statistics)
                {
                    qs.AddStatSet(StatSymbolEnum.SUM, statistic.ColName);
                }
            });
            return PartialView(model);
        }
        public IActionResult InsDataInit(string fid)
        {
            InsCase ic = _dbContext.Get<InsCase>(fid);
            IEnumerable<FapColumn> cList = _dbContext.Columns(ic.TableName);
            ViewBag.GridId = $"grid-{ic.TableName}";
            DynamicParameters param = new DynamicParameters();
            param.Add("CaseUid", ic.Fid);
            IEnumerable<InsRecord> records = _dbContext.QueryWhere<InsRecord>("CaseUid=@CaseUid and InsFlag=1", param).OrderByDescending(c => c.InsYM).OrderByDescending(c => c.InsFlag);
            if (!records.Any())
            {
                return Content("无保险记录，不用初始化");
            }
            ViewBag.Records = records;
            return PartialView(cList);
        }
        public IActionResult BaseAndRate()
        {
            JqGridViewModel model = this.GetJqGridModel("InsBaseRate");
            return View(model);
        }

        public IActionResult InsGapAnalysis(string fid)
        {
            var insRecords = _dbContext.QueryWhere<InsRecord>($"{nameof(InsRecord.CaseUid)}=@CaseUid and {nameof(InsRecord.InsFlag)}=1",
                new DynamicParameters(new { CaseUid = fid }));
            if (insRecords.Any())
            {
                return PartialView(insRecords);
            }
            return Content("还没保险记录，不能比对");
        }
    }
}