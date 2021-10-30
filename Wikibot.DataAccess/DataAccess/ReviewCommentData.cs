using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wikibot.DataAccess.Objects;

namespace Wikibot.DataAccess
{
    public class ReviewCommentData
    {
        private IDataAccess _database;

        public ReviewCommentData(IDataAccess dataAccess)
        {
            _database = dataAccess;
        }

        public void AddComment(long requestID, string comment, DateTime timestamp)
        {
            var p = new
            {
                requestId = requestID,
                comment = comment,
                timestamp = timestamp
            };

            _database.SaveData("dbo.spCreateReviewComment", p, "JobDb");
        }

        public ReviewComment GetMostRecentCommentForRequest(long requestID)
        {
            var p = new
            {
                requestId = requestID
            };

            var list = _database.LoadData<ReviewComment,dynamic>("dbo.spGetReviewCommentsForRequest", p, "JobDb");
            return list.OrderBy(x => x.TimestampUtc).Last();

        }
    }   
}
