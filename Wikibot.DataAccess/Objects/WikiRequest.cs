using System;
using System.Collections.Generic;
using System.Text;

namespace Wikibot.DataAccess.Objects
{
    public class WikiJobRequest
    {
        public long ID { get; set; }
        public string Comment { get; set; }
        public JobStatus Status { get; set; }
        public string Username { get; set; } //TODO: Make this a user object
        public DateTime SubmittedDateUTC { get; set; }
        public DateTime? TimeStartedUTC { get; set; }
        public DateTime? TimeFinishedUTC { get; set; }
        public DateTime? TimePreStartedUTC { get; set; }
        public DateTime? TimePreFinishedUTC { get; set; }
        public JobType JobType { get; set; }
        public string RawRequest { get; set; }
        public string Notes { get; set; }

        public List<Page> Pages { get; set; }
    }
}
