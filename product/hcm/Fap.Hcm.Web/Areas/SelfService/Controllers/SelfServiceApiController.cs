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
using Emp = Fap.Core.Rbac.Model.Employee;
using Fap.Core.Rbac.Model;
using DocumentFormat.OpenXml.Wordprocessing;
using Ardalis.GuardClauses;

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
                var rc = clist.Where(c => c.StartTime.ToDateTime() <= item.EndTime.ToDateTime() && c.EndTime.ToDateTime() >= item.StartTime.ToDateTime());
                if (rc.Any())
                {
                    var minTime = rc.Min(c => c.StartTime);
                    var maxTime = rc.Max(c => c.EndTime);
                    if (!results.Exists(r => r.StartTime == minTime && r.EndTime == maxTime))
                    {
                        results.Add(new EssCalendar { EventName = "占用时间", EventClass = "label-purple", StartTime = minTime, EndTime = maxTime });
                    }
                }
            }
            List<EssCalendar> terminal = new List<EssCalendar>();
            //遍历第二遍，解决三个交错并集
            foreach (var item in results)
            {
                var rc = results.Where(c => c.StartTime.ToDateTime() <= item.EndTime.ToDateTime() && c.EndTime.ToDateTime() >= item.StartTime.ToDateTime());
                if (rc.Any())
                {
                    var minTime = rc.Min(c => c.StartTime);
                    var maxTime = rc.Max(c => c.EndTime);
                    if (!terminal.Exists(r => r.StartTime == minTime && r.EndTime == maxTime))
                    {
                        terminal.Add(new EssCalendar { EventName = "占用时间", EventClass = "label-purple", StartTime = minTime, EndTime = maxTime });
                    }
                }
            }
            var list = terminal.Select(c => new { title = c.EventName, start = c.StartTime, end = c.EndTime, id = c.Id, url = c.EventUrl, allDay = false, className = c.EventClass });
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
                        new DynamicParameters(new { EventName = cal.EventName, StartTime = cal.StartTime, EndTime = cal.EndTime }));
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
        [HttpGet("ReadMessage")]
        public JsonResult ReadMessage(string fid = "")
        {
            if (fid.IsPresent())
            {
                _dbContext.Execute($"Update {nameof(FapMessage)} set {nameof(FapMessage.HasRead)}=1 where Fid=@Fid", new DynamicParameters(new { Fid = fid }));
            }
            else
            {
                _dbContext.Execute($"Update {nameof(FapMessage)} set {nameof(FapMessage.HasRead)}=1 where {nameof(FapMessage.REmpUid)}=@EmpUid", new DynamicParameters(new { EmpUid = _applicationContext.EmpUid }));
            }
            return Json(ResponseViewModelUtils.Sueecss());
        }
        [HttpGet("Partner")]
        public JsonResult MyPartner()
        {
            string sql = $"select {nameof(EssPartner.PartnerUid)} from {nameof(EssPartner)} where {nameof(EssPartner.EmpUid)} = @EmpUid and {nameof(EssPartner.Agree)}=1";
            var empUids = _dbContext.Query(sql, new DynamicParameters(new { EmpUid = _applicationContext.EmpUid })).Select(e => e.PartnerUid);
            if (empUids.Any())
            {
                sql = $"select {nameof(Emp.Fid)},{nameof(Emp.DeptUid)},{nameof(Emp.EmpPhoto)},'Offline' as OnlineState from Employee where Fid in @EmpUids";
                var emps = _dbContext.Query(sql, new DynamicParameters(new { EmpUids = empUids }), true);
                var onlineEmps = _dbContext.Query($"select {nameof(FapOnlineUser.EmpUid)} from {nameof(FapOnlineUser)} where {nameof(FapOnlineUser.OnlineState)}='{FapOnlineUser.CONST_ONLINE}' and {nameof(FapOnlineUser.EmpUid)} in @EmpUids", new DynamicParameters(new { EmpUids = empUids }))
                    .Select<dynamic, string>(e => e.EmpUid);
                foreach (var fid in onlineEmps)
                {
                    var emp = emps.FirstOrDefault(e => e.Fid == fid);
                    if (emp != null)
                    {
                        emp.OnlineState = FapOnlineUser.CONST_ONLINE;
                    }
                }
                return Json(ResponseViewModelUtils.Sueecss(emps));
            }
            return Json(ResponseViewModelUtils.Sueecss());
        }
        [HttpPost("Partner")]
        public JsonResult AddPartner(List<string> partners)
        {
            Guard.Against.Null(partners, nameof(partners));
            IList<EssPartner> list = new List<EssPartner>();
            foreach (var partnerUid in partners)
            {
                EssPartner p = new EssPartner
                {
                    EmpUid = _applicationContext.EmpUid,
                    PartnerUid = partnerUid,
                    Agree = 0
                };
                list.Add(p);
            }
            _dbContext.InsertBatchSql(list);
            return Json(ResponseViewModelUtils.Sueecss());
        }
    }
}