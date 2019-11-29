using Fap.Core.Scheduler;
using Fap.Hcm.Web;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace XUnitTestFapCore
{
    public class SchedulerTest : IClassFixture<TestWebApplicationFactory<Startup>>
    {
        private readonly ISchedulerService _schedulerService;
        public SchedulerTest(TestWebApplicationFactory<Startup> factory)
        {
            _schedulerService = factory.Services.GetService<ISchedulerService>();
        }
        [Fact]
        public void Start()
        {
            Task.Run(() =>
            _schedulerService.Run()
            );
            Task.WaitAll();
        }
    }
}
