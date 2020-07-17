using FluentScheduler;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using Wikibot.App.JobRetrievers;
using Wikibot.App.Logic;
using Wikibot.App.Models.Jobs;
using Wikibot.App.Models.UserRetrievers;

namespace Wikibot.App.Jobs
{
    public class JobRetrievalJob : AbstractJob
    {

        private IWikiJobRetriever _jobRetriever;
        private IUserRetriever _userRetriever;
        private JobContext _context;
        private DbContextOptions _options;

        public JobRetrievalJob(IConfiguration config)
        {
            Configuration = config;
            _jobRetriever = new TFWikiJobRetriever(Configuration, Site);
            _userRetriever = new TFWikiUserRetriever(Wiki);
            var builder = new SqlConnectionStringBuilder(Configuration.GetConnectionString("JobDB"));
            builder.Password = Configuration.GetSection("JobDb")["DbPassword"];
            _options = new DbContextOptionsBuilder().UseSqlServer(builder.ConnectionString).Options;
        }

        public override void Execute() {
            Console.WriteLine("Job starting");
            var jobs = _jobRetriever.JobDefinitions;
            var jobApprovalLogic = new JobApprovalLogic(_userRetriever);
            int offset = 1;
            int runin = 5;
            using (JobContext _context = new JobContext(_options))
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


                    if (jorb.Status == JobStatus.Approved)
                    {
                        //Schedule job 
                        //Scheduling logic goes here
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
            _jobRetriever.MarkJobStatuses(jobs);

        }
    }
}