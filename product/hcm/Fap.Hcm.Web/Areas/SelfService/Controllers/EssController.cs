using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Fap.AspNetCore.Infrastructure;
using Fap.AspNetCore.ViewModel;
using Fap.Core.Infrastructure.Model;
using Fap.Core.Utility;
using Microsoft.AspNetCore.Mvc;

namespace Fap.Hcm.Web.Areas.SelfService.Controllers
{
    [Area("SelfService")]
    public class EssController : FapController
    {
        public EssController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
        #region 个人日历
        /// <summary>
        /// 日常
        /// </summary>
        /// <returns></returns>
        public ActionResult WorkCalendar()
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("EmpUid", _applicationContext.EmpUid);
            IEnumerable<CfgCalendarEvent> list = _dbContext.QueryWhere<CfgCalendarEvent>("EmpUid=@EmpUid", param);
            return View(list);
        }
        #endregion

        #region 日报周报
        public ActionResult DailyWeekly()
        {
            JqGridViewModel model = this.GetJqGridModel("EssReport", (qs) =>
            {
                qs.GlobalWhere = "EmpUid='" +_applicationContext.EmpUid + "'";
                qs.AddDefaultValue("EmpUid", _applicationContext.EmpUid);
                qs.AddDefaultValue("EmpUidMC", _applicationContext.EmpName);
                qs.AddDefaultValue("DeptUid", _applicationContext.DeptUid);
                qs.AddDefaultValue("DeptUidMC", _applicationContext.DeptName);
                qs.AddDefaultValue("RptDate", DateTimeUtils.CurrentDateTimeStr);
                qs.AddOrderBy("RptDate", "desc");

            });
            return View(model);
        }
        #endregion
        public ActionResult MessageAndNotice()
        {
            JqGridViewModel model = this.GetJqGridModel("FapMessage", (qs) =>
            {
                qs.GlobalWhere = "REmpUid=@EmpUid";
                qs.AddParameter("EmpUid", _applicationContext.EmpUid);
                qs.AddOrderBy("CreateDate", "desc");
            });
            return View(model);
        }
        public ActionResult MyPartner()
        {
            MultiJqGridViewModel multi = new MultiJqGridViewModel();
            var partner = GetJqGridModel(nameof(EssPartner), qs =>
            {
                qs.GlobalWhere = "EmpUid=@EmpUid";
                qs.AddParameter("EmpUid", _applicationContext.EmpUid);
                qs.AddDefaultValue("EmpUid", _applicationContext.EmpUid);
                qs.AddDefaultValue("EmpUidMC", _applicationContext.EmpName);
            });
            multi.JqGridViewModels.Add("partner", partner);
            var partnerRequest = GetJqGridModel(nameof(EssPartner), qs =>
            {
                qs.GlobalWhere = $"{nameof(EssPartner.PartnerUid)}=@EmpUid and {nameof(EssPartner.RequestResult)}='None'";
                qs.AddParameter("EmpUid", _applicationContext.EmpUid);
            });
            multi.JqGridViewModels.Add("request", partnerRequest);
            return View(multi);
        }
    }
}