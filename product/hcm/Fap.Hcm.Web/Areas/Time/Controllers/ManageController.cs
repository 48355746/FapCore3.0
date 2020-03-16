using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fap.AspNetCore.Infrastructure;
using Fap.AspNetCore.ViewModel;
using Fap.Core.Infrastructure.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Fap.Hcm.Web.Areas.Time.Controllers
{
    [Area("Time")]
    public class ManageController : FapController
    {
        public ManageController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
        #region 基础设置
        /// <summary>
        /// 休息日设置
        /// </summary>
        /// <returns></returns>
        public ActionResult Holiday()
        {
            return View();
        }
        /// <summary>
        /// 班次设置
        /// </summary>
        /// <returns></returns>
        public ActionResult Shift()
        {
            JqGridViewModel model = GetJqGridModel("TmShift");
            return View(model);
        }
        /// <summary>
        /// 类别设置
        /// </summary>
        /// <returns></returns>
        public ActionResult TimeType()
        {
            MultiJqGridViewModel mmodel = new MultiJqGridViewModel();
            JqGridViewModel lm = GetJqGridModel("TmLeaveType");
            mmodel.JqGridViewModels.Add("leave", lm);
            JqGridViewModel tm = GetJqGridModel("TmTravelType");
            mmodel.JqGridViewModels.Add("travel", tm);
            JqGridViewModel om = GetJqGridModel("TmOvertimeType");
            mmodel.JqGridViewModels.Add("overtime", om);
            return View(mmodel);
        }
        public ActionResult Period()
        {
            var model = this.GetJqGridModel("TmPeriod", (qs) =>
            {
                qs.AddOrderBy("CurrMonth", "desc");
            });
            return View(model);

        }
        #endregion

        //GET: /Time/Manage/Schedule
        /// <summary>
        /// 排班计划表
        /// 管理员工一年的排班情况
        /// </summary>
        /// <returns></returns>
        public ActionResult Schedule()
        {
            MultiJqGridViewModel model = new MultiJqGridViewModel();
            JqGridViewModel scheduleModel = this.GetJqGridModel("TmSchedule");
            model.JqGridViewModels.Add("TmSchedule", scheduleModel);
            JqGridViewModel empModel = this.GetJqGridModel("Employee", (q) => {
                q.QueryCols = "Id,Fid,EmpCode,EmpName,EmpCategory,DeptUid,Gender,IDCard";
                q.GlobalWhere = "IsMainJob=1";
            });
            model.JqGridViewModels.Add("Employee", empModel);
            return View(model);
        }
    }
}