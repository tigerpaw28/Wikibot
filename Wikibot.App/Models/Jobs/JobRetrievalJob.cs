using System;
using Wikibot.App.JobRetrievers;
using FluentScheduler;
using System.Linq;
using System.Collections.Generic;
using LinqToWiki.Generated;
using Wikibot.App.Models.Jobs;

namespace Wikibot.App.Jobs
{
    public class JobRetrievalJob : WikiJob
    {

        private IWikiJobRetriever _jobRetriever;
        private JobContext _context;

        public JobRetrievalJob(IWikiJobRetriever jobRetriever, JobContext context)
        {
            _jobRetriever = jobRetriever;
            _context = context;
        }

        public override void Execute() {
            var jobs = _jobRetriever.JobDefinitions;
            
            foreach(WikiJob jorb in jobs)
            {
                //Check For Automatic Approval
                //ApprovalLogic.TryGetApproval(jorb);
                jorb.Status = JobStatus.PendingApproval;

                
                //Save Job
                _context.Jobs.Add(jorb);
                _context.SaveChanges();
                
                //Set JobID
                jorb.ID = _context.Jobs.AsEnumerable().Last().ID;

                
                //Update JobStatus
                //JobManager.AddJob()
                //Scheduling logic goes here
                //Schedule jobs in 5 minute intervals
                //How to deal with potential page edit overlaps? -> Check page lists and id overlaps

            }
            _jobRetriever.MarkJobStatuses(jobs);

        }
    }
}