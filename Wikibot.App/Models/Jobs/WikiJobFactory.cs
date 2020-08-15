using Microsoft.Extensions.Configuration;
using MwParserFromScratch.Nodes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Wikibot.App.JobRetrievers;
using Wikibot.App.Jobs;
using Wikibot.App.Models.Jobs;

namespace Wikibot.App.Jobs
{
    public class WikiJobFactory
    {
        public WikiJobFactory()
        {
        }
        public WikiJob GetWikiJob(JobType type, TimeZoneInfo timezone, Serilog.ILogger log, Template template = null)
        {

            var timeZoneString = GetTimeZoneString(timezone);

            WikiJob job;
            switch(type)
            {
                case JobType.TextReplacementJob:
                    job = new TextReplacementJob(log);
                    ((TextReplacementJob)job).FromText = template.Arguments.Single(arg => arg.Name.ToPlainText() == "before").Value.ToPlainText();
                    ((TextReplacementJob)job).ToText = template.Arguments.Single(arg => arg.Name.ToPlainText() == "after").Value.ToPlainText();
                    ((TextReplacementJob)job).PageNames = template.Arguments.SingleOrDefault(arg => arg.Name.ToPlainText() == "pages")?.Value.ToPlainText().Split(';').Select(val=> new Page { Name = val, ID = 0 }).ToList(); 
                    break;
                default:
                    throw new Exception("Job type is undefined");
            }

            if (template != null)
            {
                //Check for tampered with jobs
                if (template.Arguments.SingleOrDefault(arg => arg.Name.ToPlainText() == "status")?.Value.ToPlainText() == JobStatus.ToBeProcessed.ToString())
                {
                    job.Status = JobStatus.Rejected;
                }
                else
                {
                    job.Status = (JobStatus)Enum.Parse(typeof(JobStatus), template.Arguments.SingleOrDefault(arg => arg.Name.ToPlainText() == "status")?.Value.ToPlainText() ?? JobStatus.ToBeProcessed.ToString());
                }
                job.UserName = template.Arguments.Single(arg => arg.Name.ToPlainText() == "username").Value.ToPlainText();
                job.Comment = template.Arguments.Single(arg => arg.Name.ToPlainText() == "comment").Value.ToPlainText();

                //WikiMedia timestamp example: 14:58, 30 June 2020-04:00:00
                job.SubmittedDate = DateTime.ParseExact(template.Arguments.Single(arg => arg.Name.ToPlainText() == "timestamp").Value.ToPlainText().Substring(0, 19) + timeZoneString, "HH:mm, dd MMMM yyyyKKKK", new CultureInfo("en-US")).ToUniversalTime();
            }
            else
            { 
                job.Status = JobStatus.ToBeProcessed;
                job.UserName = "Wikibot";
                job.Comment = "Internally scheduled job";
                job.SubmittedDate = DateTime.UtcNow;
            }

            job.RawRequest = template.ToString();
            return job;

        }

        private string GetTimeZoneString(TimeZoneInfo timezone)
        {
            var offset = timezone.GetUtcOffset(DateTime.Now);
            var prefix = offset < TimeSpan.Zero ? "\\-" : "";
            var timeZoneFormat = prefix + "hh\\:mm";
            return offset.ToString(timeZoneFormat);

        }
    }
}
