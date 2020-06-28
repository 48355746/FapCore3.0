using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        /// 计算用户日程空闲
        /// </summary>
        /// <param name="userSchedules"></param>
        /// <returns></returns>
        [HttpPost("Calculate")]
        public IEnumerable<Schedule> CalculateSchedule(IEnumerable<UserSchedule> userSchedules)
        {
            if (!userSchedules.Any())
            {
                return Enumerable.Empty<Schedule>();
            }

            return from usches in userSchedules.Select(c => c.Schedules)
                   from us in usches
                   select us;
        }

    }
}
