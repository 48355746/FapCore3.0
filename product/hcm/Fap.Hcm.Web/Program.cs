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
                    //�����ⲿ������Fap.Core.DI.ServicesInjection����ʼ������service
                    webBuilder.UseSetting(WebHostDefaults.HostingStartupAssembliesKey, "Fap.Core")

                    .UseStartup<Startup>();
                });
    }
}
