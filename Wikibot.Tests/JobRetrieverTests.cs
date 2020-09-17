using System.Threading;
using Wikibot.DataAccess;
using Wikibot.Logic.Extensions;
using Wikibot.Logic.JobRetrievers;
using Wikibot.Logic.Jobs;
using Wikibot.Logic.Logic;
using WikiClientLibrary;
using Xunit;

namespace Wikibot.Tests
{
    public class JobRetrieverTests
    {

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
            var iConfig = Utilities.GetIConfigurationRoot();
            var retriever = new TextFileJobRetriever(iConfig, "D:\\Wikibot\\Wikibot\\WikiJobTest.txt");
            Assert.NotNull(retriever.GetNewJobDefinitions().Result);
        }

        [Fact]
        public void RunJobRetrievalJobWithTextFileRetriever()
        {
            var iConfig = Utilities.GetIConfigurationRoot();
            var jobData = new RequestData();
            var wikiAccessLogic = new WikiAccessLogic();
            var retriever = new TextFileJobRetriever(iConfig, "D:\\Wikibot\\Wikibot\\WikiJobTest.txt");
            var logger = Utilities.GetLogger(iConfig);
            var job = new JobRetrievalJob(iConfig, logger, retriever, wikiAccessLogic, jobData);
            job.Execute();
        }

        [Fact]
        public void GetNewJobDefinitionsWiki()
        {
            var iConfig = Utilities.GetIConfigurationRoot();
            var logger = Utilities.GetLogger(iConfig);
            var wikiAccessLogic = new WikiAccessLogic();
            var retriever = new TFWikiJobRetriever(iConfig, logger, wikiAccessLogic);
            Assert.NotNull(retriever.GetNewJobDefinitions().Result);
        }

        [Fact]
        public void RunJobRetrievalJobWithTFWikiRetriever()
        {
            var iConfig = Utilities.GetIConfigurationRoot();
            var jobData = new RequestData();
            var logger = Utilities.GetLogger(iConfig);
            var wikiAccessLogic = new WikiAccessLogic();
            var retriever = new TFWikiJobRetriever(iConfig, logger, wikiAccessLogic);
            var job = new JobRetrievalJob(iConfig, logger, retriever, wikiAccessLogic, jobData);
            job.Execute();
        }

        [Fact]
        public void TestSearchResults()
        {
            var iConfig = Utilities.GetIConfigurationRoot();
            var wiki = Utilities.GetWiki(iConfig);
            var results = from s in wiki.Query.search("Deceptitran")
            select new { s.title, snippet = s.snippet };
            var count1 = results.ToList().Count;
            var resultlist = results.ToList();

            var wikiSite = Utilities.GetWikiSite(iConfig);
            var results2 = wikiSite.OpenSearchAsync("Optimus Prime").Result;
            var count2 = results2.Count;

            var result3 = wikiSite.Search("{{Deceptitran", 10, BuiltInNamespaces.Main, WikiSiteExtension.SearchOptions.text, CancellationToken.None).Result;

            Assert.Equal(count1, count2);
        }
    }
}
