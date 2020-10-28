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
        private IDataAccess _database;
        public RequestData(IDataAccess dataAccess){
            _database = dataAccess;
        }
        public List<WikiJobRequest> GetWikiJobRequests()
        {
            var p = new
            {
                PageNumber = 1,
                PageSize = 10,
                SortDirection = "ASC",
                SortColumn = "ID"
            };

            var output = _database.LoadData<WikiJobRequest, dynamic>("dbo.spGetWikiJobRequests", p, "JobDb");

            return output;
        }

        public List<WikiJobRequest> GetWikiJobRequestsWithPages(int pageNumber, int pageSize, string sortDirection, string sortColumn)
        {
            var p = new
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortDirection = sortDirection,
                SortColumn = sortColumn
            };
            Type[] types = new Type[] { typeof(WikiJobRequest), typeof(Page) };

            var output = _database.LoadDataComplex<WikiJobRequest, dynamic>("dbo.spGetWikiJobRequests", p, "JobDb", types, MapPageToWikiJobRequest, "PageID");

            return output;
        }

        public WikiJobRequest GetWikiJobRequestByID(long requestID)
        {
            var p = new
            {
                RequestID = requestID
            };

            Type[] types = new Type[] { typeof(WikiJobRequest), typeof(Page) };

            var output = _database.LoadDataComplex<WikiJobRequest, dynamic>("dbo.spGetWikiJobRequestById", p, "JobDb", types, MapPageToWikiJobRequest, "PageID");
            return output.SingleOrDefault();
        }
        public void SaveWikiJobRequest(WikiJobRequest request)
        {
            var p = new DynamicParameters();
            p.Add("@Comment", request.Comment, System.Data.DbType.String);
            p.Add("@JobType", request.JobType, System.Data.DbType.String);
            p.Add("@RawRequest", request.RawRequest, System.Data.DbType.String);
            p.Add("@Status", request.Status, System.Data.DbType.Int32);
            p.Add("@SubmittedDate", request.SubmittedDateUTC, System.Data.DbType.DateTime2);
            p.Add("@Username", request.RequestingUsername, System.Data.DbType.String);
            p.Add("@ID", request.ID, System.Data.DbType.Int64, System.Data.ParameterDirection.Output);

            p.RemoveUnused = true;

            _database.SaveData("dbo.spCreateUpdateWikiJobRequest", p, "JobDb");
            request.ID = p.Get<long>("ID");
            if (request.Pages != null && request.Pages.Any())
            {    
                new PageData(_database).SavePages(request.Pages, request.ID);
            }
        }

        public void UpdateStatus(long requestID, JobStatus status)
        {
            var p = new
            {
                ID = requestID,
                Status = status
            };

            _database.SaveData("dbo.spUpdateWikiJobRequestStatus", p, "JobDb");
        }

        public void UpdateTimePreStarted(long requestID, DateTime time)
        {
            var p = new
            {
                ID = requestID,
                TimePreStartedUTC = time
            };

            _database.SaveData("dbo.spUpdateWikiJobRequestTimePreStarted", p, "JobDb");
        }

        public void UpdateTimeStarted(long requestID, DateTime time)
        {
            var p = new
            {
                ID = requestID,
                TimeStartedUTC = time
            };

            _database.SaveData("dbo.spUpdateWikiJobRequestTimeStarted", p, "JobDb");
        }

        public void UpdateTimePreFinished(long requestID, DateTime time)
        {
            var p = new
            {
                ID = requestID,
                TimePreFinishedUTC = time
            };

            _database.SaveData("dbo.spUpdateWikiJobRequestTimePreFinished", p, "JobDb");
        }

        public void UpdateTimeFinished(long requestID, DateTime time)
        {
            var p = new
            {
                ID = requestID,
                TimeFinishedUTC = time
            };

            _database.SaveData("dbo.spUpdateWikiJobRequestTimeFinished", p, "JobDb");
        }

        public void UpdateRaw(long requestID, string raw)
        {
            var p = new
            {
                ID = requestID,
                RawText = raw
            };

            _database.SaveData("dbo.spUpdateWikiJobRequestRaw", p, "JobDb");
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
