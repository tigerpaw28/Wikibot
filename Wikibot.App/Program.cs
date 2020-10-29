using FluentScheduler;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using Wikibot.DataAccess;
using Wikibot.Logic.JobRetrievers;
using Wikibot.Logic.Jobs;
using Wikibot.Logic.Logic;

namespace Wikibot.App
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
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
                    var jobRetriever = services.GetRequiredService<IWikiJobRetriever>();
                    var logger = services.GetRequiredService<ILogger>();
                    var dataAccess = services.GetRequiredService<IDataAccess>();
                    var jobData = new RequestData(dataAccess);

                    Log.Information("Starting background job retrieval job");
                    JobManager.AddJob(() => new JobRetrievalJob(config, logger, jobRetriever, wikiAccessLogic, jobData).Execute(), (s) => s.ToRunEvery(15).Minutes());
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
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                webBuilder.UseStartup<Startup>();

                });
        }
    }
}
