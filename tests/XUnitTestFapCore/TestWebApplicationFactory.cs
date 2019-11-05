using Fap.Hcm.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace XUnitTestFapCore
{
    public class TestWebApplicationFactory<TStartup> : WebApplicationFactory<Startup>
    {

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseSetting(WebHostDefaults.HostingStartupAssembliesKey, "Fap.Core").ConfigureServices(services =>
            {
                // Create a new service provider.
                var serviceProvider = new ServiceCollection()
                    // .AddEntityFrameworkInMemoryDatabase()
                    .BuildServiceProvider();
                // Add a database context (ApplicationDbContext) using an in-memory 
                // database for testing.
                // services.AddFAP();

                // Build the service provider.
                //var sp = services.BuildServiceProvider();

                // Create a scope to obtain a reference to the database
                // context (ApplicationDbContext).
                //using (var scope = sp.CreateScope())
                //{
                //    var scopedServices = scope.ServiceProvider;
                //    var db = scopedServices.GetRequiredService<CatalogContext>();
                //    var loggerFactory = scopedServices.GetRequiredService<ILoggerFactory>();

                //    var logger = scopedServices
                //        .GetRequiredService<ILogger<CustomWebRazorPagesApplicationFactory<TStartup>>>();

                //    // Ensure the database is created.
                //    db.Database.EnsureCreated();

                //    try
                //    {
                //        // Seed the database with test data.
                //        CatalogContextSeed.SeedAsync(db, loggerFactory).Wait();
                //    }
                //    catch (Exception ex)
                //    {
                //        logger.LogError(ex, $"An error occurred seeding the " +
                //            "database with test messages. Error: {ex.Message}");
                //    }
                //}
            }).UseDefaultServiceProvider(options => { options.ValidateScopes = false; });
        }
    }
}
