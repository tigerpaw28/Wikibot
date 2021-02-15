using MwParserFromScratch;
using MwParserFromScratch.Nodes;
using System;
using System.Linq;
using Wikibot.DataAccess;
using Wikibot.Logic.Factories;
using Wikibot.Logic.JobRetrievers;
using Wikibot.Logic.Jobs;
using Wikibot.Logic.Logic;
using Xunit;
using Xunit.Abstractions;

namespace Wikibot.Tests
{
    public class JobTests
    {
        private ITestOutputHelper _output;
        public JobTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void ExecuteTextReplacementJob()
        {
            var iConfig = Utilities.GetIConfigurationRoot();
            var wikiAccessLogic = new WikiAccessLogic();
            var log = Utilities.GetLogger(iConfig, _output);
            var jobData = Utilities.GetRequestData(null);
            var request = Utilities.GetSampleJobRequest();
            var jobRetriever = new TextFileJobRetriever(iConfig, "");
            TextReplacementJob job = (TextReplacementJob)WikiJobFactory.GetWikiJob(request, log, wikiAccessLogic, iConfig, jobData, jobRetriever);
            job.Configuration = iConfig;
            job.Execute();
        }

        [Fact]
        public void ExecuteLinkFixJob()
        {
            var iConfig = Utilities.GetIConfigurationRoot();
            var wikiAccessLogic = new WikiAccessLogic();
            var log = Utilities.GetLogger(iConfig, _output);
            var jobData = Utilities.GetRequestData(null);
            var request = Utilities.GetSampleLinkFixJobRequest();
            var jobRetriever = new TextFileJobRetriever(iConfig, "");
            LinkFixJob job = (LinkFixJob)WikiJobFactory.GetWikiJob(request, log, wikiAccessLogic, iConfig, jobData, jobRetriever);
            job.Configuration = iConfig;
            job.Execute();
        }

        [Fact]
        public void ExecuteContinuityLinkFixJob()
        {
            var iConfig = Utilities.GetIConfigurationRoot();
            var wikiAccessLogic = new WikiAccessLogic();
            var log = Utilities.GetLogger(iConfig, _output);
            var jobData = Utilities.GetRequestData(null);
            var request = Utilities.GetSampleContinuityLinkFixJobRequest();
            var jobRetriever = new TextFileJobRetriever(iConfig, "D:\\Wikibot\\Wikibot\\WikiJobTest.txt");
            ContinuityLinkFixJob job = (ContinuityLinkFixJob)WikiJobFactory.GetWikiJob(request, log, wikiAccessLogic, iConfig, jobData, jobRetriever);
            job.Configuration = iConfig;
            job.Execute();
        }

    }
}
