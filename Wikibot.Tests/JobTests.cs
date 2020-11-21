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

namespace Wikibot.Tests
{
    public class JobTests
    {
        [Fact]
        public void ExecuteTextReplacementJob()
        {
            var iConfig = Utilities.GetIConfigurationRoot();
            var wikiAccessLogic = new WikiAccessLogic();
            var log = Utilities.GetLogger(iConfig);
            var jobData = Utilities.GetRequestData(null);
            var request = Utilities.GetSampleJobRequest();
            var jobRetriever = new TextFileJobRetriever(iConfig, "");
            TextReplacementJob job = (TextReplacementJob)WikiJobFactory.GetWikiJob(request, log, wikiAccessLogic, iConfig, jobData, jobRetriever);
            job.Configuration = iConfig;
            job.Execute();
        }


    }
}
