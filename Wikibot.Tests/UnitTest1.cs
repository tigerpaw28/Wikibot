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
using LinqToWiki.Generated;
using System.Linq;
using Wikibot.App.Models.UserRetrievers;
using WikiClientLibrary.Sites;
using WikiClientLibrary;
using WikiClientLibrary.Client;

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
            var job = new JobRetrievalJob(iConfig);
            job.Execute();
        }

        [Fact]
        public void GetNewJobDefinitionsWiki()
        {
            var iConfig = GetIConfigurationRoot("D:\\Wikibot\\Wikibot\\Wikibot.Tests\\");
            var connectionString = iConfig.GetConnectionString("JobDb");
            var dbContextOptions = new DbContextOptionsBuilder().UseSqlServer(connectionString).Options;
            var jobContext = new JobContext(dbContextOptions);
            var retriever = new TFWikiJobRetriever(iConfig, GetWikiSite(iConfig));
            retriever.GetNewJobDefinitions().Wait();
        }

        [Fact]
        public void RunJobRetrievalJobWithTFWikiRetriever()
        {
            var iConfig = GetIConfigurationRoot("D:\\Wikibot\\Wikibot\\Wikibot.Tests\\");
            var connectionString = iConfig.GetConnectionString("JobDb");
            var dbContextOptions = new DbContextOptionsBuilder().UseSqlServer(connectionString).Options;
            var jobContext = new JobContext(dbContextOptions);
            var retriever = new TFWikiJobRetriever(iConfig, GetWikiSite(iConfig));
            var job = new JobRetrievalJob(iConfig);
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
    }
}
