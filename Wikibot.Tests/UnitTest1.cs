using System;
using Xunit;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Wikibot.App.JobRetrievers;
using Wikibot.App.Jobs;
using Wikibot.App.Models.Jobs;
using Wikibot.App.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using LinqToWiki.Generated;
using System.Linq;
using Wikibot.App.Models.UserRetrievers;
using WikiClientLibrary.Sites;
using WikiClientLibrary;
using WikiClientLibrary.Client;
using System.Collections.Generic;
using System.Threading;
using MwParserFromScratch.Nodes;
using MwParserFromScratch;
using Serilog;

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

        public static Wiki GetWiki(IConfiguration config)
        {
            var wikiLoginConfig = config.GetSection("WikiLogin");
            var username = wikiLoginConfig["Username"];
            var password = wikiLoginConfig["Password"];
            var wiki = new Wiki("WikiBot", "https://tfwiki.net", "/mediawiki/api.php");
            var result = wiki.login(username, password);

            if (result.result == loginresult.NeedToken)
                result = wiki.login(username, password, token: result.token);

            if (result.result != loginresult.Success)
                throw new Exception(result.result.ToString());


            return wiki;
        }

        public static WikiSite GetWikiSite(IConfiguration config)
        {
            var client = new WikiClient
            {
                ClientUserAgent = "WCLQuickStart/1.0 (your user name or contact information here)"
            };
            var WikiConfig = config.GetSection("WikiLogin");
            var username = WikiConfig["Username"];
            var password = WikiConfig["Password"];
            var url = WikiConfig["APIUrl"];
            // You can create multiple WikiSite instances on the same WikiClient to share the state.
            var site = new WikiSite(client, url);

            // Wait for initialization to complete.
            // Throws error if any.
            site.Initialization.Wait();
            try
            {
                site.LoginAsync(username, password).Wait();
            }
            catch (WikiClientException ex)
            {
                Console.WriteLine(ex.Message);
                // Add your exception handler for failed login attempt.
                throw;
            }
            return site;
        }

        public Serilog.ILogger GetLogger(IConfiguration config)
        {
            return new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .CreateLogger();
        }
    
        public static IUserRetriever GetUserRetriever(IConfiguration config)
        {
            var wiki = GetWiki(config);
            return new TFWikiUserRetriever(wiki);
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
            var logger = GetLogger(iConfig);
            var job = new JobRetrievalJob(iConfig, logger);
            job.Execute();
        }

        [Fact]
        public void GetNewJobDefinitionsWiki()
        {
            var iConfig = GetIConfigurationRoot("D:\\Wikibot\\Wikibot\\Wikibot.Tests\\");
            var connectionString = iConfig.GetConnectionString("JobDb");
            var dbContextOptions = new DbContextOptionsBuilder().UseSqlServer(connectionString).Options;
            var jobContext = new JobContext(dbContextOptions);
            var logger = GetLogger(iConfig);
            var retriever = new TFWikiJobRetriever(iConfig, logger, GetWikiSite(iConfig));
            retriever.GetNewJobDefinitions().Wait();
        }

        [Fact]
        public void RunJobRetrievalJobWithTFWikiRetriever()
        {
            var iConfig = GetIConfigurationRoot("D:\\Wikibot\\Wikibot\\Wikibot.Tests\\");
            var connectionString = iConfig.GetConnectionString("JobDb");
            var dbContextOptions = new DbContextOptionsBuilder().UseSqlServer(connectionString).Options;
            var jobContext = new JobContext(dbContextOptions);
            var logger = GetLogger(iConfig);
            var retriever = new TFWikiJobRetriever(iConfig, logger, GetWikiSite(iConfig));
            var job = new JobRetrievalJob(iConfig, logger);
            job.Execute();
        }

        [Fact]
        public void GetUser()
        {
            var iConfig = GetIConfigurationRoot("D:\\Wikibot\\Wikibot\\Wikibot.Tests\\");
            var userRetriever = GetUserRetriever(iConfig);
            var user = userRetriever.GetUser("Tigerpaw28");
            Assert.NotNull(user);
        }

        [Fact]
        public void GetAutoApprovedUsers()
        {
            var iConfig = GetIConfigurationRoot("D:\\Wikibot\\Wikibot\\Wikibot.Tests\\");
            var userRetriever = GetUserRetriever(iConfig);
            var users = userRetriever.GetAutoApprovedUsers();
            Assert.NotNull(users);
            Assert.NotEmpty(users);
        }

        [Fact]
        public void TestSearchResults()
        {
            var iConfig = GetIConfigurationRoot("D:\\Wikibot\\Wikibot\\Wikibot.Tests\\");
            var wiki = GetWiki(iConfig);
            var results = from s in wiki.Query.search("Deceptitran")
            select new { s.title, snippet = s.snippet };
            var count1 = results.ToList().Count;
            var resultlist = results.ToList();

            var wikiSite = GetWikiSite(iConfig);
            var results2 = wikiSite.OpenSearchAsync("Optimus Prime").Result;
            var count2 = results2.Count;

            var result3 = wikiSite.Search("{{Deceptitran", 10, BuiltInNamespaces.Main, WikiSiteExtension.SearchOptions.text, CancellationToken.None).Result;

            Assert.Equal(count1, count2);
        }

        [Fact]
        public void ExecuteTextReplacementJob()
        {
            var iConfig = GetIConfigurationRoot("D:\\Wikibot\\Wikibot\\Wikibot.Tests\\");
            var factory = new WikiJobFactory();
            var parser = new WikitextParser();
            var ast = parser.Parse("{{User:Tigerpaw28/Sandbox/Template:WikiBotRequest|type=Text Replacement|username=Tigerpaw28|timestamp=14:58, 30 June 2020 (EDT)|before=Deceptitran|after=not a Robot|comment=Test job|status=PendingPreApproval}}");
            var templates = ast.Lines.First<LineNode>().EnumDescendants().OfType<Template>();
            var log = GetLogger(iConfig);
            TextReplacementJob job = (TextReplacementJob)factory.GetWikiJob(JobType.TextReplacementJob, TimeZoneInfo.Local, log, templates.First());
            job.Configuration = iConfig;
            
            job.Execute();
        }
    }
}
