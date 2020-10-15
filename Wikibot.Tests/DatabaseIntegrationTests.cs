using Dapper;
using Moq;
using MwParserFromScratch;
using MwParserFromScratch.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var mockDataAccess = Utilities.GetMockDataAccess();
            var requestData = Utilities.GetRequestData(mockDataAccess.Object);
            var request = BuildRequest();

            var r = new DynamicParameters();
            r.Add("@Comment", request.Comment, System.Data.DbType.String);
            r.Add("@JobType", request.JobType, System.Data.DbType.String);
            r.Add("@RawRequest", request.RawRequest, System.Data.DbType.String);
            r.Add("@Status", request.Status, System.Data.DbType.Int32);
            r.Add("@SubmittedDate", request.SubmittedDateUTC, System.Data.DbType.DateTime2);
            r.Add("@Username", request.RequestingUsername, System.Data.DbType.String);
            r.Add("@ID", request.ID, System.Data.DbType.Int64, System.Data.ParameterDirection.Output);

            r.RemoveUnused = true;

            requestData.SaveWikiJobRequest(request);
            mockDataAccess.Verify(dataAccess => dataAccess.SaveData<dynamic>("dbo.spCreateWikiJobRequest", It.Is<object>(y => VerifyHelper.AreEqualObjects(y, r)), "JobDb"), Times.Exactly(1));
        }

        [Fact]
        public void GetJobs()
        {
            var mockDataAccess = Utilities.GetMockDataAccess();
            var jobList = Utilities.GetRequestData(mockDataAccess.Object).GetWikiJobRequests();
            Assert.NotEmpty(jobList);
        }

        [Fact]
        public void GetJobs2()
        {
            var mockDataAccess = Utilities.GetMockDataAccess();
            var jobList = Utilities.GetRequestData(mockDataAccess.Object).GetWikiJobRequestsWithPages(1,10,"ASC","ID");
            Assert.NotEmpty(jobList);
        }

        [Fact]
        public void GetWikiJobRequestByID()
        {
            var mockDataAccess = Utilities.GetMockDataAccess();
            var requestData = Utilities.GetRequestData(mockDataAccess.Object);
            var requestList = requestData.GetWikiJobRequestByID(2);
            Assert.NotNull(requestList);
        }

        [Fact]
        public void UpdateRequestStatus()
        {
            var mockDataAccess = Utilities.GetMockDataAccess();
            var requestData = Utilities.GetRequestData(mockDataAccess.Object);
            var requestList = requestData.GetWikiJobRequests();
            var request = requestList.First();
            request.Status = JobStatus.PreApproved;            
            requestData.UpdateStatus(request.ID, request.Status);
            //var updatedRequest = requestData.GetWikiJobRequests().First();
            //Assert.Equal(request.Status, updatedRequest.Status);
            mockDataAccess.Verify(dataAccess => dataAccess.SaveData<dynamic>("dbo.spUpdateWikiJobRequestStatus", It.Is<object>(y => VerifyHelper.AreEqualObjects(y, Utilities.GetUpdateStatusParams())), "JobDb"), Times.Exactly(1));
        }

        [Fact]
        public void UpdateRequestTimePreStarted()
        {
            var mockDataAccess = Utilities.GetMockDataAccess();
            var requestData = Utilities.GetRequestData(mockDataAccess.Object);
            var requestList = requestData.GetWikiJobRequests();
            var request = requestList.First();
            var now = DateTime.UtcNow; 
            requestData.UpdateTimePreStarted(request.ID, now);
            //var updatedRequest = requestData.GetWikiJobRequests().First();
            //Assert.Equal(request.TimePreStartedUTC.Value.ToString(), updatedRequest.TimePreStartedUTC.Value.ToString());
            mockDataAccess.Verify(dataAccess => dataAccess.SaveData<dynamic>("dbo.spUpdateWikiJobRequestTimePreStarted", It.Is<object>(y => VerifyHelper.AreEqualObjects(y, Utilities.GetTimePreStartParams(now))), "JobDb"), Times.Exactly(1));
        }

        [Fact]
        public void UpdateRequestTimeStarted()
        {
            var mockDataAccess = Utilities.GetMockDataAccess();
            var requestData = Utilities.GetRequestData(mockDataAccess.Object);
            var requestList = requestData.GetWikiJobRequests();
            var request = requestList.First();
            var now = DateTime.UtcNow;
            requestData.UpdateTimeStarted(request.ID, now);
            //var updatedRequest = requestData.GetWikiJobRequests().First();
            //Assert.Equal(request.TimeStartedUTC.ToString(), updatedRequest.TimeStartedUTC.ToString());
            mockDataAccess.Verify(dataAccess => dataAccess.SaveData<dynamic>("dbo.spUpdateWikiJobRequestTimeStarted", It.Is<object>(y => VerifyHelper.AreEqualObjects(y, Utilities.GetTimeStartParams(now))), "JobDb"), Times.Exactly(1));

        }

        [Fact]
        public void UpdateRequestTimePreFinished()
        {
            var mockDataAccess = Utilities.GetMockDataAccess();
            var requestData = Utilities.GetRequestData(mockDataAccess.Object);
            var requestList = requestData.GetWikiJobRequests();
            var request = requestList.First();
            var now = DateTime.UtcNow;
            requestData.UpdateTimePreFinished(request.ID, now);
            //var updatedRequest = requestData.GetWikiJobRequests().First();
            //Assert.Equal(request.TimePreFinishedUTC.ToString(), updatedRequest.TimePreFinishedUTC.ToString());
            mockDataAccess.Verify(dataAccess => dataAccess.SaveData<dynamic>("dbo.spUpdateWikiJobRequestTimePreFinished", It.Is<object>(y => VerifyHelper.AreEqualObjects(y, Utilities.GetTimePreFinishParams(now))), "JobDb"), Times.Exactly(1));

        }

        [Fact]
        public void UpdateRequestTimeFinished()
        {
            var mockDataAccess = Utilities.GetMockDataAccess();
            var requestData = Utilities.GetRequestData(mockDataAccess.Object);
            var requestList = requestData.GetWikiJobRequests();
            var request = requestList.First();
            var now = DateTime.UtcNow;
            requestData.UpdateTimeFinished(request.ID, now);
            //var updatedRequest = requestData.GetWikiJobRequests().First();
            //Assert.Equal(request.TimeFinishedUTC.ToString(), updatedRequest.TimeFinishedUTC.ToString());
            mockDataAccess.Verify(dataAccess => dataAccess.SaveData<dynamic>("dbo.spUpdateWikiJobRequestTimeFinished", It.Is<object>(y => VerifyHelper.AreEqualObjects(y, Utilities.GetTimeFinishParams(now))), "JobDb"), Times.Exactly(1));

        }

        [Fact]
        public void UpdatePages()
        {
            var mockDataAccess = Utilities.GetMockDataAccess();
            var requestData = Utilities.GetRequestData(mockDataAccess.Object);
            var requestList = requestData.GetWikiJobRequests();
            var request = requestList.First();
            List<Page> pages = new List<Page>();
            pages.Add(new Page(0, "NewPage"));
            var pageData = new PageData(mockDataAccess.Object);
            pageData.UpdatePagesForWikiJobRequest(pages, request.ID);
            mockDataAccess.Verify(dataAccess => dataAccess.SaveData<dynamic>("dbo.spUpdatePagesForWikiJobRequest", It.Is<object>(y => VerifyHelper.AreEqualObjects(y, Utilities.GetUpdatePageParams(pages,request.ID))), "JobDb"), Times.Exactly(1));
        }
    }
}
