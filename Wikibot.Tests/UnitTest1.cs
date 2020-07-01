using System;
using Xunit;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Wikibot.App.JobRetrievers;
using Wikibot.App.Jobs;
using Wikibot.App.Models.Jobs;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using Microsoft.Extensions.Configuration;

namespace Wikibot.Tests
{
    public class JobRetrieverTests
    {
        public static IConfigurationRoot GetIConfigurationRoot(string outputPath)
        {
            return new ConfigurationBuilder()
                .SetBasePath(outputPath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddUserSecrets("e3dfcccf-0cb3-423a-b302-e3e92e95c128")
                .AddEnvironmentVariables()
                .Build();
        }

        // [Fact]
        // public void Startup()
        // {
        //     String[] args = new string[0];
        //     var hostBuilder = Host.CreateDefaultBuilder(args)
        //         .ConfigureWebHostDefaults(webBuilder =>
        //         {
        //             webBuilder.UseStartup<Startup>();
        //         });
        //     //var startup = new Startup(config);
        //     Assert.NotNull(hostBuilder.Build());
        // }

        [Fact]
        public void GetNewJobDefinitionsTextFile()
        {
            var iConfig = GetIConfigurationRoot("D:\\Wikibot\\Wikibot.Tests\\");
            var connectionString = iConfig.GetConnectionString("JobDb");
            var dbContextOptions = new DbContextOptionsBuilder().UseSqlServer(connectionString).Options;
            var jobContext = new JobContext(dbContextOptions);
            var retriever = new TextFileJobRetriever(jobContext, iConfig, "D:\\Wikibot\\WikiJobTest.txt");
            Assert.NotNull(retriever.GetNewJobDefinitions().Result);
        }

        [Fact]
        public void RunJobRetrievalJobWithTextFileRetriever()
        {
            var iConfig = GetIConfigurationRoot("D:\\Wikibot\\Wikibot.Tests\\");
            var connectionString = iConfig.GetConnectionString("JobDb");
            var dbContextOptions = new DbContextOptionsBuilder().UseSqlServer(connectionString).Options;
            var jobContext = new JobContext(dbContextOptions);
            var retriever = new TextFileJobRetriever(jobContext, iConfig, "D:\\Wikibot\\WikiJobTest.txt");
            var job = new JobRetrievalJob(retriever, jobContext);
            job.Execute();
        }

        [Fact]
        public void GetNewJobDefinitionsWiki()
        {
            var iConfig = GetIConfigurationRoot("D:\\Wikibot\\Wikibot.Tests\\");
            var connectionString = iConfig.GetConnectionString("JobDb");
            var dbContextOptions = new DbContextOptionsBuilder().UseSqlServer(connectionString).Options;
            var jobContext = new JobContext(dbContextOptions);
            var retriever = new TFWikiJobRetriever(jobContext, iConfig);
            retriever.GetNewJobDefinitions().Wait();
        }

        [Fact]
        public void RunJobRetrievalJobWithTFWikiRetriever()
        {
            var iConfig = GetIConfigurationRoot("D:\\Wikibot\\Wikibot.Tests\\");
            var connectionString = iConfig.GetConnectionString("JobDb");
            var dbContextOptions = new DbContextOptionsBuilder().UseSqlServer(connectionString).Options;
            var jobContext = new JobContext(dbContextOptions);
            var retriever = new TFWikiJobRetriever(jobContext, iConfig);
            var job = new JobRetrievalJob(retriever, jobContext);
            job.Execute();
        }
    }
}
