using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Wikibot.Logic.Extensions;
using Wikibot.DataAccess.Objects;
using System.Linq;
using Dapper;

namespace Wikibot.DataAccess
{
    public class PageData
    {
        public void SavePages(List<Page> pageList, long jobRequestID)
        {
            SqlDataAccess sql = new SqlDataAccess();
            var p = new
            {
                pages = pageList.ToList().ToDataSet().Tables[0].AsTableValuedParameter("PageUDT"),
                jobid = jobRequestID
            };

            sql.SaveData("dbo.spCreatePages", p, "JobDb");
        }

        public void UpdatePagesForWikiJobRequest(List<Page> pageList, long jobRequestID)
        {
            SqlDataAccess sql = new SqlDataAccess();
            var p = new
            {
                pages = pageList.ToList().ToDataSet().Tables[0].AsTableValuedParameter("PageUDT"),
                jobid = jobRequestID
            };

            sql.SaveData("dbo.spUpdatePagesForWikiJobRequest", p, "JobDb");
        }
    }
}
