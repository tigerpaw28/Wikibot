using Dapper;
using LinqToWiki;
using LinqToWiki.Generated;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;
using Moq;
using MwParserFromScratch;
using MwParserFromScratch.Nodes;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using Wikibot.DataAccess;
using Wikibot.DataAccess.Objects;
using Wikibot.Logic.Extensions;
using Wikibot.Logic.Factories;
using Wikibot.Logic.UserRetrievers;
using WikiClientLibrary;
using WikiClientLibrary.Client;
using WikiClientLibrary.Sites;

namespace Wikibot.Tests
{
    public static class Utilities
    {
        const string CONFIGURATION_ROOT_PATH = "D:\\Wikibot\\Wikibot\\Wikibot.Tests\\";

        public static IConfigurationRoot GetIConfigurationRoot()
        {
            return new ConfigurationBuilder()
                .SetBasePath(CONFIGURATION_ROOT_PATH)
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

        public static Serilog.ILogger GetLogger(IConfiguration config)
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

        public static WikiJobRequest GetSampleJobRequest()
        {
            var parser = new WikitextParser();
            var ast = parser.Parse("{{User:Tigerpaw28/Sandbox/Template:WikiBotRequest|type=Text Replacement|username=Tigerpaw28|timestamp=14:58, 30 June 2020 (EDT)|before=Deceptitran|after=not a Robot|comment=Test job|status=PendingPreApproval}}");
            var templates = ast.Lines.First<LineNode>().EnumDescendants().OfType<Template>();
            var request = WikiJobRequestFactory.GetWikiJobRequest(JobType.TextReplacementJob, TimeZoneInfo.Local, templates.First());
            return request;
        }

        public static List<WikiJobRequest> GetSampleJobRequests(bool includePages)
        {
            var requests = new List<WikiJobRequest>();
            var array = GetRawRequestArray();
            var parser = new WikitextParser();
            for (int x = 0; x < 10; x++)
            {
                var ast = parser.Parse(array[0]);
                var templates = ast.Lines.First<LineNode>().EnumDescendants().OfType<Template>();
                var request = WikiJobRequestFactory.GetWikiJobRequest(JobType.TextReplacementJob, TimeZoneInfo.Local, templates.First());
                request.ID = x + 2;
                if(!includePages)
                {
                    request.Pages = null;
                }
                requests.Add(request);
            }

            return requests;
        }

        private static string[] GetRawRequestArray()
        {
            string[] raws = new string[10];
            raws[0] = "{{deceptitran| before =<nowiki> W:User_talk:</nowiki> | after =<nowiki> http://www.wikia.com/wiki/User_talk: </nowiki> | pages = Commercial; Commercial / Japan; Transformers_(2019_comic) | username =[[User: Tigerpaw28 | Tigerpaw28]] | timestamp = 17:54, 17 September 2009 (EDT)| comment = The Wikia link removal(at least I presume this to be the culprit) created a whole bunch of invalid talk page links on talk pages, which are now in the Wanted Pages list. Removing the link mark-up while still indicating what the link was, will work too.So long as we get them off the Wanted list.}}";
            raws[1] = "{{deceptitran| before = Optimus Prime| after = Orion Pax| pages = Optimus_Prime_(G1)| username =[[User: Tigerpaw28 | Tigerpaw28]] | timestamp = 17:54, 17 September 2009 (EDT)| comment = That's just Prime.}}";
            raws[2] = "{{deceptitran| before = Megatron| after = Galvatron| pages = Megatron_(G1)| username =[[User: Tigerpaw28 | Tigerpaw28]]| timestamp = 17:54, 17 September 2009 (EDT)| comment = Behold.}}";
            raws[3] = "{{deceptitran| before = Blaster| after = Twincast| pages = Twincast_(G1)| username =[[User: Tigerpaw28 | Tigerpaw28]]| timestamp = 17:54, 17 September 2009 (EDT)| comment = He's back.}}";
            raws[4] = "{{deceptitran| before = Soundwave| after = Soundblaster| pages = Soundblaster_(G1)| username =[[User: Tigerpaw28 | Tigerpaw28]]| timestamp = 17:54, 17 September 2009 (EDT)| comment = Soundwave not superior.}}";
            return raws;
        }

        public static Mock<IDataAccess> GetMockDataAccess()
        {
            var mock = new Mock<IDataAccess>();
            var request = GetSampleJobRequest();
            request.ID = 2;

            var ldcParams = new
            {
                RequestID = request.ID
            };

            var ldParams = new
            {
                PageNumber = 1,
                PageSize = 10,
                SortDirection = "ASC",
                SortColumn = "ID"
            };

            var sdParams = new DynamicParameters();
            sdParams.Add("@Comment", request.Comment, System.Data.DbType.String);
            sdParams.Add("@JobType", request.JobType, System.Data.DbType.String);
            sdParams.Add("@RawRequest", request.RawRequest, System.Data.DbType.String);
            sdParams.Add("@Status", request.Status, System.Data.DbType.Int32);
            sdParams.Add("@SubmittedDate", request.SubmittedDateUTC, System.Data.DbType.DateTime2);
            sdParams.Add("@Username", request.RequestingUsername, System.Data.DbType.String);
            sdParams.Add("@ID", request.ID, System.Data.DbType.Int64, System.Data.ParameterDirection.Output);

            sdParams.RemoveUnused = true;

            var updateStatus = GetUpdateStatusParams();

            var updateTimePreStart = GetTimePreStartParams(DateTime.UtcNow);

            var updateTimeStart = GetTimeStartParams(DateTime.UtcNow);

            var updateTimePreFinish = GetTimePreFinishParams(DateTime.UtcNow);

            var updateTimeFinish = GetTimeFinishParams(DateTime.UtcNow);
            
            var pages = new List<DataAccess.Objects.Page>();
            pages.Add(new DataAccess.Objects.Page(0, "NewPage"));
            var updatePages = GetUpdatePageParams(pages, request.ID);


            Type[] types = new Type[] { typeof(WikiJobRequest), typeof(DataAccess.Objects.Page) };

            //Instruct the mock
            mock.Setup(dataAccess => dataAccess.LoadDataComplex<WikiJobRequest, dynamic>("dbo.spGetWikiJobRequestById", It.Is<object>(y => VerifyHelper.AreEqualObjects(y, ldcParams)), "JobDb", types, It.IsAny<Func<object[], WikiJobRequest>>(), "PageID")).Returns(new List<WikiJobRequest> { request });
            mock.Setup(dataAccess => dataAccess.LoadData<WikiJobRequest, dynamic>("dbo.spGetWikiJobRequests", It.Is<object>(y => VerifyHelper.AreEqualObjects(y, ldParams)), "JobDb")).Returns(GetSampleJobRequests(false));
            mock.Setup(dataAccess => dataAccess.LoadDataComplex<WikiJobRequest, dynamic>("dbo.spGetWikiJobRequests", It.Is<object>(y => VerifyHelper.AreEqualObjects(y, ldParams)), "JobDb", types, It.IsAny<Func<object[], WikiJobRequest>>(), "PageID")).Returns(GetSampleJobRequests(true));
            mock.Setup(dataAccess => dataAccess.LoadDataComplex<WikiJobRequest, dynamic>("dbo.spGetWikiJobRequestsForApproval", It.Is<object>(y => VerifyHelper.AreEqualObjects(y, ldParams)), "JobDb", types, It.IsAny<Func<object[], WikiJobRequest>>(), "PageID")).Returns(GetSampleJobRequests(true).Where(x=> x.Status == JobStatus.PendingPreApproval || x.Status == JobStatus.PendingApproval).ToList());
            mock.Setup(dataAccess => dataAccess.SaveData<dynamic>("dbo.spCreateWikiJobRequest", It.Is<object>(y => VerifyHelper.AreEqualObjects(y, sdParams)), "JobDb"));
            mock.Setup(dataAccess => dataAccess.SaveData<dynamic>("dbo.spUpdateWikiJobRequestStatus", It.Is<object>(y => VerifyHelper.AreEqualObjects(y, updateStatus)), "JobDb"));
            mock.Setup(dataAccess => dataAccess.SaveData<dynamic>("dbo.spUpdateWikiJobRequestTimePreStarted", It.Is<object>(y => VerifyHelper.AreEqualObjects(y, updateTimePreStart)), "JobDb"));
            mock.Setup(dataAccess => dataAccess.SaveData<dynamic>("dbo.spUpdateWikiJobRequestTimeStarted", It.Is<object>(y => VerifyHelper.AreEqualObjects(y, updateTimeStart)), "JobDb"));
            mock.Setup(dataAccess => dataAccess.SaveData<dynamic>("dbo.spUpdateWikiJobRequestTimePreFinished", It.Is<object>(y => VerifyHelper.AreEqualObjects(y, updateTimePreFinish)), "JobDb"));
            mock.Setup(dataAccess => dataAccess.SaveData<dynamic>("dbo.spUpdateWikiJobRequestTimeFinished", It.Is<object>(y => VerifyHelper.AreEqualObjects(y, updateTimeFinish)), "JobDb"));
            mock.Setup(dataAccess => dataAccess.SaveData<dynamic>("dbo.spCreatePages", It.IsAny<List<DataAccess.Objects.Page>>(), "JobDb"));
            mock.Setup(dataAccess => dataAccess.SaveData<dynamic>("dbo.spUpdatePagesForWikiJobRequest", It.Is<object>(y => VerifyHelper.AreEqualObjects(y, updatePages)), "JobDb"));
            var result = mock.Object.LoadDataComplex<WikiJobRequest, dynamic>("dbo.spGetWikiJobRequestById", ldcParams, "JobDb", types, MapPageToWikiJobRequest, "PageID");
            mock.Verify(dataAccess => dataAccess.LoadDataComplex<WikiJobRequest, dynamic>("dbo.spGetWikiJobRequestById", ldcParams, "JobDb", types, MapPageToWikiJobRequest, "PageID"));

            return mock;

        }

        public static object GetUpdateStatusParams()
        {
            return new
            {
                ID = 2,
                Status = JobStatus.PreApproved
            };
        }

        public static object GetTimePreStartParams(DateTime now)
        {
            return new
            {
                ID = 2,
                TimePreStartedUTC = now
            };
        }

        public static object GetTimeStartParams(DateTime now)
        {
            return new
            {
                ID = 2,
                TimeStartedUTC = now
            };
        }

        public static object GetTimePreFinishParams(DateTime now)
        {
            return new
            {
                ID = 2,
                TimePreFinishedUTC = now
            };
        }

        public static object GetTimeFinishParams(DateTime now)
        {
            return new
            {
                ID = 2,
                TimeFinishedUTC = now
            };
        }

        public static object GetUpdatePageParams(List<DataAccess.Objects.Page> pageList, long id)
        {
            return new {
                pages = pageList.ToList().ToDataSet().Tables[0].AsTableValuedParameter("PageUDT"),
                jobid = id
            };
        }

        public static RequestData GetRequestData(IDataAccess dataAccess)
        {
            if(dataAccess == null)
            {
                dataAccess = GetMockDataAccess().Object;
            }
            var requestData = new RequestData(dataAccess);
            
            return requestData;
        }

        private static WikiJobRequest MapPageToWikiJobRequest(object[] obj)
        {
            var request = (WikiJobRequest)obj[0];
            var page = (DataAccess.Objects.Page)obj[1];

            if (request.Pages == null)
            {
                request.Pages = new List<DataAccess.Objects.Page>();
            }

            if (page != null)
            {
                if (!request.Pages.Any(x => x.PageID == page.PageID))
                {
                    request.Pages.Add(page);
                }
            }

            return request;
        }

    }
}
