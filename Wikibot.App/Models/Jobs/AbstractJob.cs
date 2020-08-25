using LinqToWiki.Generated;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Wikibot.App.Data;
using WikiClientLibrary;
using WikiClientLibrary.Client;
using WikiClientLibrary.Sites;

namespace Wikibot.App.Jobs
{
    public class AbstractJob
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        public string Comment { get; set; }
        public JobStatus Status { get; set; }
        public string UserName { get; set; } //TODO: Make this a user object
        public DateTime SubmittedDate { get; set; }
        public DateTime TimeStarted { get; set; }
        public DateTime TimeFinished { get; set; }
        public DateTime TimePreStarted { get; set; }
        public DateTime TimePreFinished { get; set; }
        
        [NotMapped]
        public IConfiguration Configuration { get; set; }

        [NotMapped]
        public Serilog.ILogger Log { get; set; }
        public virtual void Execute() { }

        private DbContextOptions _dboptions;
        public DbContextOptions DBOptions
        {
            get
            {
                if(_dboptions == null)
                {
                    var builder = new SqlConnectionStringBuilder(Configuration.GetConnectionString("JobDB"));
                    builder.Password = Configuration.GetSection("JobDb")["DbPassword"];
                    _dboptions = new DbContextOptionsBuilder().UseSqlServer(builder.ConnectionString).Options;
                }
                return _dboptions;
            }
        }

        public void SetJobStart()
        {
            if (Status == JobStatus.PreApproved)
            {
                TimePreStarted = DateTime.UtcNow;
                Log.Information("Job started at {DateTime}", TimePreStarted);
            }
            else
            {
                TimeStarted = DateTime.UtcNow;
                Log.Information("Job started at {DateTime}", TimeStarted);
            }
        }

        public void SetJobEnd()
        {
            if (Status == JobStatus.PreApproved)
            {
                TimePreFinished = DateTime.UtcNow;
                Status = JobStatus.PendingApproval;
                Log.Information("Job ended at {DateTime}", TimePreFinished);
            }
            else if(Status == JobStatus.Approved)
            {
                TimeFinished = DateTime.UtcNow;
                Status = JobStatus.Done;
                Log.Information("Job ended at {DateTime}", TimeFinished);
            }

        }

        public void SaveJob()
        {
            Log.Information("Saving job.");
            using (JobContext context = new JobContext(DBOptions))
            {
                context.Jobs.Update((WikiJob)this);
                context.SaveChanges();
            }
        }
    }
}
