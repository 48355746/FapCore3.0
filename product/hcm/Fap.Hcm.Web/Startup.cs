using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Fap.Core.Infrastructure.Domain;
using Fap.Core.DI;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Cookies;
using Newtonsoft.Json;
using Fap.Hcm.Web.Models;

namespace Fap.Hcm.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddFapService().AddAutoInjection().AddDataTracker();
            //���schedule��̨����
            services.AddHostedService<BackgroundSchedulerService>();

            //response ѹ��giz
            services.AddResponseCompression();
            //����ڴ滺��
            services.AddMemoryCache();
            services.AddSession();
            //�����֤
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromHours(1);
                options.LoginPath = new PathString("/");
                options.LogoutPath = "/";
                //options.AccessDeniedPath = "";//ָ��·������ return Forbid() or ForbidAsync()
                options.Cookie = new CookieBuilder
                {
                    IsEssential = false // required for auth to work without explicit user consent; adjust to suit your privacy policy
                };
            });
            services.AddMiniProfiler(options =>
            {
                options.PopupRenderPosition = StackExchange.Profiling.RenderPosition.BottomLeft;
                options.PopupShowTimeWithChildren = true;
                options.RouteBasePath = "/profiler";
            });
            services.AddControllersWithViews().AddNewtonsoftJson().AddRazorRuntimeCompilation();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // force the en-US culture, so that the app behaves the same even on machines with different default culture
            var supportedCultures = new[] { new CultureInfo("en-US") };
            //Ӧ�����ݸ�������ӦAddDataTracker();
            app.UseDataTracker();
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-US"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //���ɶ���js�ļ�
                app.BuilderMultiLanguageJsFile();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseResponseCompression();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();
            //��ʱ�ļ��й�����ʹ��
            app.UseStaticFiles(new StaticFileOptions()
            {
                ServeUnknownFileTypes = true,
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), $"{FapPlatformConstants.TemporaryFolder}")),
                RequestPath = new PathString($"/{FapPlatformConstants.TemporaryFolder}")
            });
            app.UseRouting();
            //��֤
            app.UseAuthentication();
            //��Ȩ
            app.UseAuthorization();
            app.UseMiniProfiler();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "areaRoute",
                    pattern: "{area:exists}/{controller}/{action}",
                    defaults: new { action = "Index" });

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{fid?}");
            });
        }
    }
}
