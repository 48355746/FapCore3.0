using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Fap.Hcm.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    //增加外部启动项Fap.Core.DI.ServicesInjection，初始化所有service
                    webBuilder.UseSetting(WebHostDefaults.HostingStartupAssembliesKey, "Fap.Core")

                    .UseStartup<Startup>();
                });
    }
}
