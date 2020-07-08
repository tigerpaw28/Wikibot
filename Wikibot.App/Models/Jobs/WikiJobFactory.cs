using Microsoft.Extensions.Configuration;
using MwParserFromScratch.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wikibot.App.JobRetrievers;
using Wikibot.App.Jobs;
using Wikibot.App.Models.Jobs;

namespace Wikibot.App.Jobs
{
    public enum JobType
    {
        JobRetrievalJob,
        TextReplacementJob
    }
    public class WikiJobFactory
    {
        private JobContext _context;
        public WikiJobFactory(JobContext context)
        {
            _context = context;
        }
        public WikiJob GetWikiJob(JobType type, Template template = null)
        {
            WikiJob job;
            switch(type)
            {
                case JobType.TextReplacementJob:
                    job = new TextReplacementJob();
                    ((TextReplacementJob)job).FromText = template.Arguments.Single(arg => arg.Name.ToPlainText() == "before").Value.ToPlainText();
                    ((TextReplacementJob)job).ToText = template.Arguments.Single(arg => arg.Name.ToPlainText() == "after").Value.ToPlainText();
                    ((TextReplacementJob)job).PageNames = template.Arguments.SingleOrDefault(arg => arg.Name.ToPlainText() == "pages")?.Value.ToPlainText().Split(';').ToList();
                    break;
                default:
                    throw new Exception("Job type is undefined");
            }

            if (template != null)
            {
                job.Status = (JobStatus)Enum.Parse(typeof(JobStatus),template.Arguments.SingleOrDefault(arg => arg.Name.ToPlainText() == "status")?.Value.ToPlainText() ?? JobStatus.ToBeProcessed.ToString());
                job.UserName = template.Arguments.Single(arg => arg.Name.ToPlainText() == "username").Value.ToPlainText();
                job.Comment = template.Arguments.Single(arg => arg.Name.ToPlainText() == "comment").Value.ToPlainText();
            }
            else
            {
                job.Status = JobStatus.ToBeProcessed;
                job.UserName = "Wikibot";
                job.Comment = "Internally scheduled job";
            }

            job.RawRequest = template.ToString();
            return job;

        }
    }
}
