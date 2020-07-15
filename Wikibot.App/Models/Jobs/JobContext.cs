using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wikibot.App.Jobs;

namespace Wikibot.App.Models.Jobs
{
    public class JobContext : DbContext
    {
        public JobContext(DbContextOptions options) : base(options) { }

        public DbSet<WikiJob> Jobs { get; set; }
        public DbSet<TextReplacementJob> TextReplacementJobs { get; set; }
    }
}
