using FluentScheduler;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using Wikibot.App.JobRetrievers;
using Wikibot.App.Logic;
using Wikibot.App.Models.Jobs;
using Wikibot.App.Models.UserRetrievers;
using Serilog;

namespace Wikibot.App.Jobs
{
    public class JobRetrievalJob : AbstractJob
    {

        private IWikiJobRetriever _jobRetriever;
        private IUserRetriever _userRetriever;

        public JobRetrievalJob(IConfiguration config, Serilog.ILogger log)
        {
            Configuration = config;
            Log = log;
            _jobRetriever = new TFWikiJobRetriever(Configuration, Log, Site);
            _userRetriever = new TFWikiUserRetriever(Wiki);          
        }

        public override void Execute() {
            Log.Information("Job retrieval job starting");

            var jobApprovalLogic = new JobApprovalLogic(_userRetriever);
            int offset = 1;
            int runin = 5;

            //Get job definitions
            var jobs = _jobRetriever.JobDefinitions;

            using (JobContext _context = new JobContext(DBOptions))
            {
                foreach (WikiJob jorb in jobs)
                {
                    //Check For Automatic Approval
                    var user = _userRetriever.GetUser(jorb.UserName);

                    if (jobApprovalLogic.IsUserAutoApproved(user))
                    {
                        jorb.Status = JobStatus.PreApproved;
                    }
                    else
                        jorb.Status = JobStatus.PendingPreApproval;


                    //Save Job
                    _context.Jobs.Add(jorb);
                    _context.SaveChanges();

                    //Set JobID
                    jorb.ID = _context.Jobs.AsEnumerable().Last().ID;


                    if (jorb.Status == JobStatus.Approved || jorb.Status == JobStatus.PreApproved)
                    {
                        //Schedule jobs in 5 minute intervals
                        //How to deal with potential page edit overlaps? -> Check page lists and id overlaps;
                        if (offset == 5)
                        {
                            runin = runin + offset;
                            offset = 0;
                        }
                        JobManager.AddJob(() => jorb.Execute(), (s) => s.ToRunOnceIn(runin).Minutes());
                        offset++;
                    }
                }  
            }

            //Update job status on the wiki page the job was retreived from
            _jobRetriever.MarkJobStatuses(jobs);

        }
    }
}