using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Wikibot.App.Models.Jobs;

namespace Wikibot.App.Jobs
{
    public class WikiJob : IWikiJob
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }

        public string Comment { get; set; }
        public JobStatus Status { get; set; }
        public JobType RequestType { get; set; }
        public string RawRequest { get; set; }
        public string UserName { get; set; } //TODO: Make this a user object
        public DateTime SubmittedDate { get; set; }
        public DateTime TimePreStarted { get; set; }
        public DateTime TimePreFinished { get; set; }
        public DateTime TimeStarted { get; set; }
        public DateTime TimeFinished { get; set; }
        public string ProposedChanges { get; set; }
        public string Notes { get; set; }

        public virtual void Execute() { }
    }
}
