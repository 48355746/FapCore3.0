using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Fap.Hcm.WebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Fap.Hcm.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ScheduleController : ControllerBase
    {
        private readonly ILogger<ScheduleController> _logger;

        public ScheduleController(ILogger<ScheduleController> logger)
        {
            _logger = logger;
        }
        /// <summary>
        /// 活动时间匹配
        /// </summary>
        /// <param name="activity">活动</param>
        /// <returns></returns>
        [HttpPost("Calculate")]
        public IEnumerable<IdleSchedule> CalculateSchedule(Activity activity)
        {
            Guard.Against.Null(activity, nameof(activity));
            var sp = activity.EndDate - activity.StartDate;
            int days = sp.Days + 1;
            if (!activity.UserSchedules.Any())
            {
                return Enumerable.Range(0, days).Select(d => new IdleSchedule
                {
                    StartDateTime = Convert.ToDateTime($"{activity.StartDate.AddDays(d).ToString("yyyy-MM-dd")} {activity.StartTime}"),
                    EndDateTime = Convert.ToDateTime($"{activity.StartDate.AddDays(d).ToString("yyyy-MM-dd")} {activity.EndTime}")
                });
            }
            var schedules = from usches in activity.UserSchedules.Select(c => c.Schedules)
                            from us in usches
                            select us;
            IEnumerable<IdleSchedule> result = Calculate();
            if (!result.Any())
            {
                schedules= schedules.Where(d => d.Priority == true);
                return Calculate();
            }
            return result;
            IEnumerable<IdleSchedule> Calculate()
            {
                foreach (var day in Enumerable.Range(0, days))
                {
                    var sd = Convert.ToDateTime($"{activity.StartDate.AddDays(day).ToString("yyyy-MM-dd")} {activity.StartTime}");
                    var ed = Convert.ToDateTime($"{activity.StartDate.AddDays(day).ToString("yyyy-MM-dd")} {activity.EndTime}");
                    //得到今天的所有日程
                    var currSchedules = schedules.Where(sch => sch.StartDateTime <= ed && sch.EndDateTime >= sd);
                    if (!currSchedules.Any())
                    {
                        yield return new IdleSchedule
                        {
                            StartDateTime = sd,
                            EndDateTime = ed
                        };
                    }
                    else
                    {
                        var colls = ScheduleCollection(currSchedules).OrderBy(s => s.StartDateTime);                       
                        var schedules = colls.ToArray();
                        if (schedules.Length > 1)
                        {
                            for (int i = 1; i < colls.Count();i++)
                            {
                                if (schedules[i].StartDateTime < sd||schedules[i].StartDateTime>ed)
                                {
                                    continue;
                                }
                                var st = schedules[i].StartDateTime;
                                var et = schedules[i - 1].EndDateTime;
                                if (et < sd)
                                {
                                    et = sd;
                                }
                                if (st > ed)
                                {
                                    st = ed;
                                }
                                if ((st - et).TotalHours >=activity.Duration)
                                {
                                    yield return new IdleSchedule { StartDateTime = st, EndDateTime = et };
                                }
                            }
                          
                        }
                        var firstH = colls.First();
                        var lastH = colls.Last();
                        if (firstH.StartDateTime > sd && (firstH.StartDateTime - sd).TotalHours > activity.Duration)
                        {
                            yield return new IdleSchedule { StartDateTime = sd, EndDateTime = firstH.StartDateTime };
                        }
                        if (lastH.EndDateTime < ed && (ed - lastH.EndDateTime).TotalHours > activity.Duration)
                        {
                            yield return new IdleSchedule { StartDateTime = lastH.EndDateTime, EndDateTime = ed };
                        }                        

                    }
                }
            }
            List<Schedule> ScheduleCollection(IEnumerable<Schedule> currSchedules)
            {
                List<Schedule> results = new List<Schedule>();
                foreach (var item in currSchedules)
                {
                    var rc = currSchedules.Where(c => c.StartDateTime <= item.EndDateTime && c.EndDateTime >= item.StartDateTime);
                    if (rc.Any())
                    {
                        var minTime = rc.Min(c => c.StartDateTime);
                        var maxTime = rc.Max(c => c.EndDateTime);
                        if (!results.Exists(r => r.StartDateTime == minTime && r.EndDateTime == maxTime))
                        {
                            results.Add(new Schedule { StartDateTime = minTime, EndDateTime = maxTime });
                        }
                    }
                }
                List<Schedule> terminal = new List<Schedule>();
                //遍历第二遍，解决三个交错并集
                foreach (var item in results)
                {
                    var rc = results.Where(c => c.StartDateTime <= item.EndDateTime && c.EndDateTime >= item.StartDateTime);
                    if (rc.Any())
                    {
                        var minTime = rc.Min(c => c.StartDateTime);
                        var maxTime = rc.Max(c => c.EndDateTime);
                        if (!terminal.Exists(r => r.StartDateTime == minTime && r.EndDateTime == maxTime))
                        {
                            terminal.Add(new Schedule { StartDateTime = minTime, EndDateTime = maxTime });
                        }
                    }
                }
                return terminal;
            }
        }

    }
}
