﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wikibot.DataAccess;

namespace Wikibot.App.Models
{
    public class WikiJobRequest
    {
        public long ID { get; set; }
        public string Comment { get; set; }
        public string StatusName { get; set; }
        public string RequestingUsername { get; set; } //TODO: Make this a user object
        public DateTime SubmittedDateUTC { get; set; }
        public DateTime? TimePreStartedUTC { get; set; }
        public JobType JobType { get; set; }
        public string RawRequest { get; set; }
        public string Notes { get; set; }
        public List<string> Diffs { get; set; }

    }
}