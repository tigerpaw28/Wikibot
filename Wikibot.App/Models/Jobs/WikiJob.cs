using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

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

        public virtual void Execute() { }
    }
}
