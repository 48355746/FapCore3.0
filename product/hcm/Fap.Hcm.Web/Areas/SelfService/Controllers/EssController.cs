using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Fap.AspNetCore.Infrastructure;
using Fap.Core.Infrastructure.Model;
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
    }
}