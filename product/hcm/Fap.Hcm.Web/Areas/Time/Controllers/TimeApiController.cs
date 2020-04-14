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
        public TimeApiController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _timeService = serviceProvider.GetService<ITimeService>();
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
        [HttpGet("Period")]
        public JsonResult TmPeriod()
        {
            var periods = _dbContext.QueryAll<TmPeriod>().OrderByDescending(p => p.CurrMonth).Take(12);
            return Json(periods);
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
        /// <summary>
        /// 排班
        /// </summary>
        /// <param name="schedule"></param>
        /// <returns></returns>
        [HttpPost("Schedule")]
        public JsonResult PostInitSchedule(ScheduleViewModel schedule)
        {
            var pageable = _gridFormService.AnalysisPostData(schedule.PostData);
            var empList = _dbContext.Query<Fap.Core.Rbac.Model.Employee>(pageable.ToString());
            if (!empList.Any())
            {
                return Json(ResponseViewModelUtils.Failure("请设置排班员工"));
            }
            _timeService.Schedule(empList, schedule.ShiftUid, schedule.HolidayUid, schedule.StartDate, schedule.EndDate);
            return Json(ResponseViewModelUtils.Sueecss());
        }
        /// <summary>
        /// 批量打卡
        /// </summary>
        /// <param name="batchCard"></param>
        /// <returns></returns>
        [HttpPost("BatchCard")]
        public JsonResult PostBatchCard(BatchCardViewModel batchCard)
        {
            Guard.Against.Null(batchCard, nameof(batchCard));
            _timeService.BatchPatchCard(batchCard.DeptUidList, batchCard.StartDate, batchCard.EndDate);
            return Json(ResponseViewModelUtils.Sueecss());
        }
        [HttpPost("AnnualLeave/Init")]
        public JsonResult AnnualLeaveInit(string year, string startDate, string endDate)
        {
            _timeService.AnnualLeaveInit(year, startDate, endDate);
            return Json(ResponseViewModelUtils.Sueecss());
        }
        [HttpPost("AnnualLeave/Surplus")]
        public JsonResult AnnualLeaveSurplus(string year, string lastYear)
        {
            _timeService.AnnualLeaveSurplus(year, lastYear);
            return Json(ResponseViewModelUtils.Sueecss());
        }
        [HttpGet("DayResult/Calulate")]
        public JsonResult DayResultCalulate()
        {
            _timeService.DayResultCalculate();
            return Json(ResponseViewModelUtils.Sueecss());
        }
        [HttpGet("MonthResult/Calulate")]
        public JsonResult MonthResultCalulate()
        {
            _timeService.MonthResultCalculate();
            return Json(ResponseViewModelUtils.Sueecss());
        }
        [HttpPost("Exception/Notice")]
        public JsonResult ExceptionNotice(string[] options)
        {
            if (options.Length > 0)
            {
                _timeService.ExceptionNotice(options);
            }
            return Json(ResponseViewModelUtils.Sueecss());
        }
        [HttpPost("LeavelDays")]
        public JsonResult LeavelDays(string empUid, string startDateTime, string endDateTime)
        {
            var (days, hours) = _timeService.LeavelDays(empUid, startDateTime, endDateTime);
            return Json(new ResponseViewModel { data = new { days = days, hours = hours },success=true });
        }
        [HttpGet("Annual/ValidDays")]
        public JsonResult AnnualValidDays(string empUid,string startTime)
        {
            double days = _timeService.ValidAnnualLeaveDays(empUid, startTime);
            return Json(new ResponseViewModel { success = true, data = days });
        }
        [HttpGet("Overtime/ValidDays")]
        public JsonResult OvertimeValidDays(string empUid)
        {
            double hours=_timeService.OvertimeValidHours(empUid);
            return Json(new ResponseViewModel { success = true, data = hours });
        }
    }
}