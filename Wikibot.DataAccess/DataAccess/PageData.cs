using Dapper;
using System.Collections.Generic;
using System.Linq;
using Wikibot.DataAccess.Objects;
using Wikibot.Logic.Extensions;

namespace Wikibot.DataAccess
{
    public class PageData
    {
        private IDataAccess _database;
        public PageData(IDataAccess dataAccess)
        {
            _database = dataAccess;
        }

        public void SavePages(List<Page> pageList, long jobRequestID)
        {
            var p = new
            {
                pages = pageList.ToList().ToDataSet().Tables[0].AsTableValuedParameter("PageUDT"),
                jobid = jobRequestID
            };

            _database.SaveData("dbo.spCreatePages", p, "JobDb");
        }

        public void UpdatePagesForWikiJobRequest(List<Page> pageList, long jobRequestID)
        {
            var p = new
            {
                pages = pageList.ToList().ToDataSet().Tables[0].AsTableValuedParameter("PageUDT"),
                jobid = jobRequestID
            };

            _database.SaveData("dbo.spUpdatePagesForWikiJobRequest", p, "JobDb");
        }
    }
}
