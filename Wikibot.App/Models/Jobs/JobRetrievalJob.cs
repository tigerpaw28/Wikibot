using System;
using Wikibot.App.JobRetrievers;
using FluentScheduler;
using System.Linq;
using System.Collections.Generic;
using LinqToWiki.Generated;
using Wikibot.App.Models.Jobs;
using Wikibot.App.Models.UserRetrievers;
using Wikibot.App.Logic;

namespace Wikibot.App.Jobs
{
    public class JobRetrievalJob : WikiJob
    {

        private IWikiJobRetriever _jobRetriever;
        private IUserRetriever _userRetriever;
        private JobContext _context;

        public JobRetrievalJob(IWikiJobRetriever jobRetriever, IUserRetriever userRetriever, JobContext context)
        {
            _jobRetriever = jobRetriever;
            _userRetriever = userRetriever;
            _context = context;
        }

        public override void Execute() {
            var jobs = _jobRetriever.JobDefinitions;
            var jobApprovalLogic = new JobApprovalLogic(_userRetriever);

            foreach(WikiJob jorb in jobs)
            {
                //Check For Automatic Approval
                var user = _userRetriever.GetUser(jorb.UserName);

                if (jobApprovalLogic.IsUserAutoApproved(user))
                    jorb.Status = JobStatus.Approved;
                else
                    jorb.Status = JobStatus.PendingApproval;

                
                //Save Job
                _context.Jobs.Add(jorb);
                _context.SaveChanges();
                
                //Set JobID
                jorb.ID = _context.Jobs.AsEnumerable().Last().ID;

                
                if(jorb.Status == JobStatus.Approved)
                {
                    //Schedule job 
                    //Scheduling logic goes here
                    //Schedule jobs in 5 minute intervals
                    //How to deal with potential page edit overlaps? -> Check page lists and id overlaps
                }
            }
            _jobRetriever.MarkJobStatuses(jobs);

        }
    }
}