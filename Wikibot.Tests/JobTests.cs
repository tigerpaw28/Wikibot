using MwParserFromScratch;
using MwParserFromScratch.Nodes;
using System;
using System.Linq;
using Wikibot.DataAccess;
using Wikibot.Logic.Factories;
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
            var parser = new WikitextParser();
            var ast = parser.Parse("{{User:Tigerpaw28/Sandbox/Template:WikiBotRequest|type=Text Replacement|username=Tigerpaw28|timestamp=14:58, 30 June 2020 (EDT)|before=Deceptitran|after=not a Robot|comment=Test job|status=PendingPreApproval}}");
            var templates = ast.Lines.First<LineNode>().EnumDescendants().OfType<Template>();
            var wikiAccessLogic = new WikiAccessLogic();
            var log = Utilities.GetLogger(iConfig);
            var jobData = new RequestData();
            var request = WikiJobRequestFactory.GetWikiJobRequest(JobType.TextReplacementJob, TimeZoneInfo.Local, templates.First());
            TextReplacementJob job = (TextReplacementJob)WikiJobFactory.GetWikiJob(request, log, wikiAccessLogic, iConfig, jobData);
            job.Configuration = iConfig;
            job.Execute();
        }


    }
}
