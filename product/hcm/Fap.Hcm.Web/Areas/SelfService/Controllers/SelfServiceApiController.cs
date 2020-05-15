using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Fap.AspNetCore.Infrastructure;
using Fap.AspNetCore.ViewModel;
using Fap.Core.Infrastructure.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Fap.Core.Utility;
using Fap.Core.Extensions;
using Dapper;
using Fap.Hcm.Service.Organization;
using DocumentFormat.OpenXml.Drawing;

namespace Fap.Hcm.Web.Areas.SelfService.Controllers
{
    [Area("SelfService")]
    [Produces(MediaTypeNames.Application.Json)]
    [Route("[area]/Api")]
    public class SelfServiceApiController : FapController
    {
        private readonly IOrganizationService _organizationService;
        public SelfServiceApiController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _organizationService = serviceProvider.GetService<IOrganizationService>();
        }
        [HttpPost("CalendarEvent")]
        public JsonResult CalendarEvent(CfgCalendarEvent cevent)
        {
            cevent.EmpUid = _applicationContext.EmpUid;
            cevent.IsGlobal = 0;
            _dbContext.Insert(cevent);
            return Json(ResponseViewModelUtils.Sueecss(cevent));
        }
        [HttpDelete("CalendarEvent/{fids}")]
        public JsonResult CalendarEvent(string fids)
        {
            var fs = fids.SplitComma();
            string sql = "delete from CfgCalendarEvent where Fid in @Fids";
            DynamicParameters param = new DynamicParameters();
            param.Add("Fids", fs);
            _dbContext.Execute(sql, param);
            return Json(ResponseViewModelUtils.Sueecss());
        }
        /// <summary>
        /// 获取员工所有事件
        /// </summary>
        /// <returns></returns>
        [HttpGet("Calendars")]
        public JsonResult GetCalendar(string start, string end)
        {
            string startDate = DateTimeUtils.DateTimeFormat(DateTimeUtils.ToDateTime(start).AddSeconds(-1));
            string endDate = DateTimeUtils.DateTimeFormat(DateTimeUtils.ToDateTime(end).AddDays(1));

            DynamicParameters param = new DynamicParameters();
            param.Add("EmpUid", _applicationContext.EmpUid);
            param.Add("StartTime", startDate);
            param.Add("EndTime", endDate);
            var clist = _dbContext.QueryWhere<EssCalendar>("EmpUid=@EmpUid and StartTime>@StartTime and StartTime<@EndTime", param);
            var list = clist.Select(c => new { title = c.EventName, start = c.StartTime, end = c.EndTime, id = c.Id, url = c.EventUrl, allDay = c.IsAllDay.ToBool(), className = c.EventClass });
            return JsonIgnoreNull(list);
        }
        [HttpPost("DeptCalendars")]
        public JsonResult DeptCalendars(string start, string end, List<string> empUids)
        {
            string startDate = DateTimeUtils.DateTimeFormat(DateTimeUtils.ToDateTime(start).AddSeconds(-1));
            string endDate = DateTimeUtils.DateTimeFormat(DateTimeUtils.ToDateTime(end).AddDays(1));

            DynamicParameters param = new DynamicParameters();
            param.Add("EmpUids", empUids);
            param.Add("StartTime", startDate);
            param.Add("EndTime", endDate);
            var clist = _dbContext.QueryWhere<EssCalendar>("EmpUid in @EmpUids and StartTime>@StartTime and StartTime<@EndTime", param, true);
            var list = clist.Select(c => new { title = $"{c.EmpUidMC}:{c.EventName}", start = c.StartTime, end = c.EndTime, id = c.Id, url = c.EventUrl, allDay = c.IsAllDay.ToBool(), className = c.EventClass });
            return JsonIgnoreNull(list);
        }
        [HttpPost("DeptCalendarMatch")]
        public JsonResult DeptCalendarMatch(string start, string end, List<string> empUids)
        {
            string startDate = DateTimeUtils.DateTimeFormat(DateTimeUtils.ToDateTime(start).AddSeconds(-1));
            string endDate = DateTimeUtils.DateTimeFormat(DateTimeUtils.ToDateTime(end).AddDays(1));

            DynamicParameters param = new DynamicParameters();
            param.Add("EmpUids", empUids);
            param.Add("StartTime", startDate);
            param.Add("EndTime", endDate);
            var clist = _dbContext.QueryWhere<EssCalendar>("EmpUid in @EmpUids and StartTime>@StartTime and StartTime<@EndTime and EndTime!=''", param, true);
            List<EssCalendar> results = new List<EssCalendar>();
            foreach (var item in clist)
            {
                var rc= clist.Where(c => c.StartTime.ToDateTime() <= item.EndTime.ToDateTime() && c.EndTime.ToDateTime() >= item.StartTime.ToDateTime());
                if (rc.Any())
                {
                    var minTime= rc.Min(c => c.StartTime);
                    var maxTime = rc.Max(c => c.EndTime);
                    if (!results.Exists(r => r.StartTime == minTime && r.EndTime == maxTime))
                    {
                        results.Add(new EssCalendar { EventName = "占用时间", EventClass = "label-purple", StartTime = minTime, EndTime = maxTime });
                    }
                }
            }
            var list = results.Select(c => new { title =c.EventName, start = c.StartTime, end = c.EndTime, id = c.Id, url = c.EventUrl, allDay =false, className = c.EventClass });
            return JsonIgnoreNull(list);
        }
        
        /// <summary>
        /// 保存员工日历事件
        /// </summary>
        /// <param name="calendar"></param>
        /// <returns></returns>
        [HttpPost("Calendar")]
        public JsonResult PostSaveCalendar(EssCalendar calendar)
        {
            calendar.EmpUid = _applicationContext.EmpUid;//
            var cal = _dbContext.Get<EssCalendar>(calendar.Id);
            if (calendar.Dr == 1)
            {
                if (cal != null)
                {
                    _dbContext.Delete<EssCalendar>(cal);
                }
                return Json(new { success = true });
            }
            if (calendar.Id == 0)
            {
                var newC = _dbContext.Insert<EssCalendar>(calendar);
                return JsonIgnoreNull(calendar);
            }
            else
            {
                cal.EventName = calendar.EventName;
                cal.EventClass = calendar.EventClass;
                cal.StartTime = calendar.StartTime;
                cal.EndTime = calendar.EndTime;
                cal.IsAllDay = calendar.IsAllDay;
                var updateC = _dbContext.Update<EssCalendar>(cal);
                return JsonIgnoreNull(cal);
            }
        }
        [HttpPost("DeptCalendar")]
        public JsonResult PostSaveDeptCalendar(EssCalendar calendar)
        {
            if (calendar.Dr == 1)
            {
                var cal = _dbContext.Get<EssCalendar>(calendar.Id);
                if (cal != null)
                {
                    _dbContext.DeleteExec(nameof(EssCalendar), $"{nameof(EssCalendar.EventName)}=@EventName and {nameof(EssCalendar.StartTime)}=@StartTime and {nameof(EssCalendar.EndTime)}=@EndTime and {nameof(EssCalendar.Origin)}='dept'",
                        new DynamicParameters(new { EventName= cal.EventName, StartTime=cal.StartTime, EndTime=cal.EndTime }));
                }
                return Json(new { success = true });
            }
            if (calendar.Id == 0)
            {
                calendar.EmpUid = _applicationContext.EmpUid;//
                List<EssCalendar> addList = new List<EssCalendar>();
                foreach (var fid in calendar.EmpUids)
                {
                    var c = calendar.Clone() as EssCalendar;
                    c.EmpUid = fid;
                    addList.Add(c);
                }
                _dbContext.InsertBatchSql<EssCalendar>(addList);
                _dbContext.Insert(calendar);
                return JsonIgnoreNull(calendar);
            }
            return Json(new { success = true });
        }

        [HttpGet("DominationDepartment")]
        public JsonResult DominationDepartment()
        {
            var tree = _organizationService.GetDominationDepartmentTree();
            return Json(tree);
        }
    }
}