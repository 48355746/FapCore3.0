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
        public IEnumerable<Schedule> CalculateSchedule(Activity activity)
        {
            Guard.Against.Null(activity, nameof(activity));
            if (!activity.UserSchedules.Any())
            {
                var sp= activity.EndDate - activity.StartDate;
                return Enumerable.Range(0, sp.Days + 1).Select(d => new Schedule
                {
                    StartDateTime = Convert.ToDateTime($"{activity.StartDate.AddDays(d)} {activity.StartTime}"),
                    EndDateTime= Convert.ToDateTime($"{activity.StartDate.AddDays(d)} {activity.EndTime}")
                });
            }

            return from usches in activity.UserSchedules.Select(c => c.Schedules)
                   from us in usches
                   select us;
        }

    }
}
