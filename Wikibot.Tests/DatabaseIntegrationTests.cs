using MwParserFromScratch;
using MwParserFromScratch.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wikibot.DataAccess;
using Wikibot.DataAccess.Objects;
using Wikibot.Logic.Factories;
using Xunit;

namespace Wikibot.Tests
{
    public class DatabaseIntegrationTests
    {
        private WikiJobRequest BuildRequest()
        {
            var parser = new WikitextParser();
            var ast = parser.Parse("{{User:Tigerpaw28/Sandbox/Template:WikiBotRequest|type=Text Replacement|username=Tigerpaw28|timestamp=14:58, 30 June 2020 (EDT)|before=Deceptitran|after=not a Robot|comment=Test job|status=PendingPreApproval}}");
            var templates = ast.Lines.First<LineNode>().EnumDescendants().OfType<Template>();
            var request = WikiJobRequestFactory.GetWikiJobRequest(JobType.TextReplacementJob, TimeZoneInfo.Local, templates.First());
            List<Page> pages = new List<Page>();
            pages.Add(new Page(0, "Test"));
            pages.Add(new Page(1, "Commercial"));
            request.Pages = pages;
            return request;
        }

        [Fact]
        public void SaveRequest()
        {
            var requestData = new RequestData();
            var request = BuildRequest();
            requestData.SaveWikiJobRequest(request);
        }

        [Fact]
        public void GetJobs()
        {
            var jobList = new RequestData().GetWikiJobRequests();
            Assert.NotEmpty(jobList);
        }

        [Fact]
        public void GetJobs2()
        {
            var jobList = new RequestData().GetWikiJobRequestsWithPages();
            Assert.NotEmpty(jobList);
        }

        [Fact]
        public void GetWikiJobRequestByID()
        {
            var jobList = new RequestData().GetWikiJobRequestByID(2);
            Assert.NotEmpty(jobList);
        }

        [Fact]
        public void UpdateRequestStatus()
        {
            var requestData = new RequestData();
            var requestList = requestData.GetWikiJobRequests();
            var request = requestList.First();
            request.Status = JobStatus.Approved;            
            requestData.UpdateWikiJobRequestStatus(request.ID, request.Status);
            var updatedRequest = requestData.GetWikiJobRequests().First();
            Assert.Equal(request.Status, updatedRequest.Status);
        }

        [Fact]
        public void UpdateRequestTimePreStarted()
        {
            var requestData = new RequestData();
            var requestList = requestData.GetWikiJobRequests();
            var request = requestList.First();
            request.TimePreStartedUTC = DateTime.UtcNow;
            requestData.UpdateWikiJobRequestTimePreStarted(request.ID, request.TimePreStartedUTC.Value);
            var updatedRequest = requestData.GetWikiJobRequests().First();
            Assert.Equal(request.TimePreStartedUTC.Value.ToString(), updatedRequest.TimePreStartedUTC.Value.ToString());
        }

        [Fact]
        public void UpdateRequestTimeStarted()
        {
            var requestData = new RequestData();
            var requestList = requestData.GetWikiJobRequests();
            var request = requestList.First();
            request.TimeStartedUTC = DateTime.UtcNow;
            requestData.UpdateWikiJobRequestTimeStarted(request.ID, request.TimeStartedUTC.Value);
            var updatedRequest = requestData.GetWikiJobRequests().First();
            Assert.Equal(request.TimeStartedUTC.ToString(), updatedRequest.TimeStartedUTC.ToString());
        }

        [Fact]
        public void UpdateRequestTimePreFinished()
        {
            var requestData = new RequestData();
            var requestList = requestData.GetWikiJobRequests();
            var request = requestList.First();
            request.TimePreFinishedUTC = DateTime.UtcNow;
            requestData.UpdateWikiJobRequestTimePreFinished(request.ID, request.TimePreFinishedUTC.Value);
            var updatedRequest = requestData.GetWikiJobRequests().First();
            Assert.Equal(request.TimePreFinishedUTC.ToString(), updatedRequest.TimePreFinishedUTC.ToString());
        }

        [Fact]
        public void UpdateRequestTimeFinished()
        {
            var requestData = new RequestData();
            var requestList = requestData.GetWikiJobRequests();
            var request = requestList.First();
            request.TimeFinishedUTC = DateTime.UtcNow;
            requestData.UpdateWikiJobRequestTimeFinished(request.ID, request.TimeFinishedUTC.Value);
            var updatedRequest = requestData.GetWikiJobRequests().First();
            Assert.Equal(request.TimeFinishedUTC.ToString(), updatedRequest.TimeFinishedUTC.ToString());
        }


        [Fact]
        public void GetPages()
        {
            var requestData = new RequestData();
            var requestList = requestData.GetWikiJobRequestByID(2);
            var request = requestList.First();
            List<Page> pages = new List<Page>();
            pages.Add(new Page(0, "NewPage"));
            var pageData = new PageData();
            pageData.UpdatePagesForWikiJobRequest(pages, request.ID);
            var updatedRequest = requestData.GetWikiJobRequestByID(2).Single();
            Assert.Equal(pages.Select(x=> x.Name), updatedRequest.Pages.Select(x=> x.Name));
        }

        [Fact]
        public void UpdatePages()
        {
            var requestData = new RequestData();
            var requestList = requestData.GetWikiJobRequests();
            var request = requestList.First();
            List<Page> pages = new List<Page>();
            pages.Add(new Page(0, "NewPage"));
            var pageData = new PageData();
            pageData.UpdatePagesForWikiJobRequest(pages, request.ID);
        }
    }
}
