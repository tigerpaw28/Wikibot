using FluentScheduler;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using Wikibot.DataAccess;
using Wikibot.Logic;
using Wikibot.Logic.JobRetrievers;
using Wikibot.Logic.Jobs;
using Wikibot.Logic.Logic;
using Wikibot.Logic.UserRetrievers;

namespace Wikibot.App
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddUserSecrets("aspnet-Wikibot.App-3FB00538-5AEC-40E7-8DBC-0BF9B37C229B")
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            try
            {
                var host = CreateHostBuilder(args).Build();

                using (var serviceScope = host.Services.CreateScope())
                {
                    var services = serviceScope.ServiceProvider;
                    var config = services.GetRequiredService<IConfiguration>();
                    var wikiAccessLogic = services.GetRequiredService<IWikiAccessLogic>();
                    var userRetriever = services.GetRequiredService<IUserRetriever>();
                    var jobRetriever = services.GetRequiredService<IWikiRequestRetriever>();
                    var logger = services.GetRequiredService<ILogger>();
                    var dataAccess = services.GetRequiredService<IDataAccess>();
                    var notifier = services.GetRequiredService<INotificationService>();
                    var jobData = new RequestData(dataAccess);
                    var welcomeInterval = config.GetValue<int>("WelcomeInterval");

                    Log.Information("Starting background job retrieval job");
                    JobManager.AddJob(() => new RequestRetrievalJob(config, logger, jobRetriever, userRetriever, notifier, jobData).Execute(), (s) => s.ToRunEvery(15).Minutes());
                    JobManager.AddJob(() => new WelcomeJob(wikiAccessLogic, config, logger).Execute(false), (s) => s.ToRunEvery(welcomeInterval).Minutes());
                }
                
                Log.Information("Application Start");
                host.Run();
            }
            catch(Exception ex)
            {
                Log.Fatal(ex, "Wikibot failed to start correctly.");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;

                    config.AddJsonFile("appsettings.json")
                          .AddUserSecrets("aspnet-Wikibot.App-3FB00538-5AEC-40E7-8DBC-0BF9B37C229B")
                          .AddJsonFile($"appsettings.{env.EnvironmentName}.json",
                                         optional: true, reloadOnChange: true);
                })
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                webBuilder.UseStartup<Startup>();

                });
        }
    }
}
