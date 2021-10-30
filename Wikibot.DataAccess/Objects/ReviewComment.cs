using System;

namespace Wikibot.DataAccess.Objects
{
    public class ReviewComment
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime TimestampUtc { get; set; }
    }
}
