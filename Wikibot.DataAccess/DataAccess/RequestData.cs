using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wikibot.DataAccess.Objects;

namespace Wikibot.DataAccess
{
    public class RequestData
    {
        public List<WikiJobRequest> GetWikiJobRequests()
        {
            SqlDataAccess sql = new SqlDataAccess();

            var p = new
            {
                PageNumber = 1,
                PageSize = 10,
                SortDirection = "ASC",
                SortColumn = "ID"
            };

            var output = sql.LoadData<WikiJobRequest, dynamic>("dbo.spGetWikiJobRequests", p, "JobDb");

            return output;
        }

        public List<WikiJobRequest> GetWikiJobRequestsWithPages()
        {
            SqlDataAccess sql = new SqlDataAccess();

            var p = new
            {
                PageNumber = 1,
                PageSize = 10,
                SortDirection = "ASC",
                SortColumn = "ID"
            };
            Type[] types = new Type[] { typeof(WikiJobRequest), typeof(Page) };

            var output = sql.LoadData2<WikiJobRequest, dynamic>("dbo.spGetWikiJobRequests", p, "JobDb", types, MapPageToWikiJobRequest, "PageID");

            return output;
        }

        public List<WikiJobRequest> GetWikiJobRequestByID(long requestID)
        {
            SqlDataAccess sql = new SqlDataAccess();

            var p = new
            {
                RequestID = requestID
            };

            Type[] types = new Type[] { typeof(WikiJobRequest), typeof(Page) };


            var output = sql.LoadData2<WikiJobRequest, dynamic>("dbo.spGetWikiJobRequestById", p, "JobDb", types, MapPageToWikiJobRequest, "PageID");
            return output;
        }
        public void SaveWikiJobRequest(WikiJobRequest request)
        {
            SqlDataAccess sql = new SqlDataAccess();
            var p = new DynamicParameters();
            p.Add("@Comment", request.Comment, System.Data.DbType.String);
            p.Add("@JobType", request.JobType, System.Data.DbType.String);
            p.Add("@RawRequest", request.RawRequest, System.Data.DbType.String);
            p.Add("@Status", request.Status, System.Data.DbType.Int32);
            p.Add("@SubmittedDate", request.SubmittedDateUTC, System.Data.DbType.DateTime2);
            p.Add("@Username", request.Username, System.Data.DbType.String);
            p.Add("@ID", request.ID, System.Data.DbType.Int64, System.Data.ParameterDirection.Output);

            p.RemoveUnused = true;

            sql.SaveData("dbo.spCreateWikiJobRequest", p, "JobDb");
            request.ID = p.Get<long>("ID");
            new PageData().SavePages(request.Pages, request.ID);
        }

        public void UpdateWikiJobRequestStatus(long requestID, JobStatus status)
        {
            SqlDataAccess sql = new SqlDataAccess();

            var p = new
            {
                ID = requestID,
                Status = status
            };

            sql.SaveData("dbo.spUpdateWikiJobRequestStatus", p, "JobDb");
        }

        public void UpdateWikiJobRequestTimePreStarted(long requestID, DateTime time)
        {
            SqlDataAccess sql = new SqlDataAccess();

            var p = new
            {
                ID = requestID,
                TimePreStartedUTC = time
            };

            sql.SaveData("dbo.spUpdateWikiJobRequestTimePreStarted", p, "JobDb");
        }

        public void UpdateWikiJobRequestTimeStarted(long requestID, DateTime time)
        {
            SqlDataAccess sql = new SqlDataAccess();

            var p = new
            {
                ID = requestID,
                TimeStartedUTC = time
            };

            sql.SaveData("dbo.spUpdateWikiJobRequestTimeStarted", p, "JobDb");
        }

        public void UpdateWikiJobRequestTimePreFinished(long requestID, DateTime time)
        {
            SqlDataAccess sql = new SqlDataAccess();

            var p = new
            {
                ID = requestID,
                TimePreFinishedUTC = time
            };

            sql.SaveData("dbo.spUpdateWikiJobRequestTimePreFinished", p, "JobDb");
        }

        public void UpdateWikiJobRequestTimeFinished(long requestID, DateTime time)
        {
            SqlDataAccess sql = new SqlDataAccess();

            var p = new
            {
                ID = requestID,
                TimeFinishedUTC = time
            };

            sql.SaveData("dbo.spUpdateWikiJobRequestTimeFinished", p, "JobDb");
        }

        private WikiJobRequest MapPageToWikiJobRequest(object[] obj)
        {
            var request = (WikiJobRequest)obj[0];
            var page = (Page)obj[1];

            if (request.Pages == null)
            {
                request.Pages = new List<Page>();
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
