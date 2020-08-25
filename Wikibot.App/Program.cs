using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentScheduler;
using LinqToWiki.Generated;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Wikibot.App.JobRetrievers;
using Wikibot.App.Jobs;
using Wikibot.App.Data;
using Wikibot.App.Models.UserRetrievers;
using WikiClientLibrary.Sites;

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
                    var config = host.Services.GetRequiredService<IConfiguration>();

                    Log.Information("Starting background job retrieval job");
                    JobManager.AddJob(() => new JobRetrievalJob(config, Log.Logger).Execute(), (s) => s.ToRunEvery(15).Minutes());
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
            //.ConfigureLogging((context, logging) => {
            //    logging.ClearProviders();
            //    logging.AddConfiguration(context.Configuration.GetSection("Logging"));
            //    logging.AddDebug();
            //    logging.AddConsole();
            //})
                .ConfigureWebHostDefaults(webBuilder =>
                {
                webBuilder.UseStartup<Startup>();

                });
        }
    }
}
