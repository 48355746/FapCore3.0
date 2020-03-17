using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Fap.AspNetCore.Infrastructure;
using Fap.Hcm.Service.Time;
using Fap.Hcm.Web.Areas.Time.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Fap.AspNetCore.ViewModel;
using Dapper;
using Fap.AspNetCore.Serivce;

namespace Fap.Hcm.Web.Areas.Time.Controllers
{
    [Area("Time")]
    [Produces(MediaTypeNames.Application.Json)]
    [Route("[area]/Api")]
    public class TimeApiController : FapController
    {
        private readonly ITimeService _timeService;
        private readonly IGridFormService _gridFormService;
        public TimeApiController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _timeService = serviceProvider.GetService<ITimeService>();
            _gridFormService = serviceProvider.GetService<IGridFormService>();
        }
        [HttpPost("Holiday")]
        public JsonResult PostHoliday(HolidayViewModel holiday)
        {
            Guard.Against.Null(holiday, nameof(holiday));
            IList<TmHoliday> list = new List<TmHoliday>();
            if (holiday.Holidays != null)
            {
                foreach (var day in holiday.Holidays)
                {
                    TmHoliday model = new TmHoliday();
                    model.CaseUid = holiday.CaseUid;
                    model.Holiday = day;
                    DayOfWeek dw = Convert.ToDateTime(day).DayOfWeek;
                    if (dw == DayOfWeek.Sunday)
                    {
                        model.HodidayNote = "星期天";
                    }
                    else if (dw == DayOfWeek.Saturday)
                    {
                        model.HodidayNote = "星期六";
                    }
                    else
                    {
                        model.HodidayNote = "节假日";
                    }
                    list.Add(model);
                }
            }
            _timeService.AddHoliday(holiday.CaseUid, list);
            return Json(ResponseViewModelUtils.Sueecss());
        }
        /// <summary>
        /// 获取假日
        /// </summary>
        /// <param name="caseUid"></param>
        /// <returns></returns>
        [HttpGet("Holiday/{caseUid=''}")]
        public JsonResult GetHoliday(string caseUid)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("CaseUid", caseUid);
            IEnumerable<TmHoliday> list = _dbContext.QueryWhere<TmHoliday>("CaseUid=@CaseUid", param);
            var holiday = list.Select(l => l.Holiday);
            return Json(holiday);
        }
        [HttpPost("Schedule")]
        public JsonResult PostInitSchedule(ScheduleViewModel schedule)
        {
            var pageable= _gridFormService.AnalysisPostData(schedule.PostData);
            var empList = _dbContext.Query(pageable.ToString());
            //ReturnMsg rmsg = _timeService.InitSchedule(strWhere, schedule.ShiftUid, schedule.HolidayUid, schedule.StartDate, schedule.EndDate);
            return Json(ResponseViewModelUtils.Sueecss());
        }
    }
}