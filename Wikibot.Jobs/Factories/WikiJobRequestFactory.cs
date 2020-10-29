using MwParserFromScratch.Nodes;
using System;
using System.Globalization;
using System.Linq;
using Wikibot.DataAccess;
using Wikibot.DataAccess.Objects;

namespace Wikibot.Logic.Factories
{
    public static class WikiJobRequestFactory
    {
        public static WikiJobRequest GetWikiJobRequest(JobType type, TimeZoneInfo timezone, Template template = null)
        {

            var timeZoneString = GetTimeZoneString(timezone);
            WikiJobRequest jobRequest = new WikiJobRequest();

            switch (type)
            {
                case JobType.TextReplacementJob:
                    jobRequest.JobType = JobType.TextReplacementJob;
                    //((TextReplacementJob)job).FromText = template.Arguments.Single(arg => arg.Name.ToPlainText() == "before").Value.ToPlainText();
                    //((TextReplacementJob)job).ToText = template.Arguments.Single(arg => arg.Name.ToPlainText() == "after").Value.ToPlainText();
                    jobRequest.Pages = template.Arguments.SingleOrDefault(arg => arg.Name.ToPlainText() == "pages")?.Value.ToPlainText().Split(';').Select(val => new Page { Name = val, PageID = 0 }).ToList();
                    break;
                default:
                    throw new Exception("Job type is undefined");
            }

            if (template != null)
            {
                jobRequest.Status = (JobStatus)Enum.Parse(typeof(JobStatus), template.Arguments.SingleOrDefault(arg => arg.Name.ToPlainText() == "status")?.Value.ToPlainText() ?? JobStatus.ToBeProcessed.ToString());
                jobRequest.ID = long.Parse(template.Arguments.SingleOrDefault(arg => arg.Name.ToPlainText() == "id")?.Value.ToPlainText() ?? "0");
                jobRequest.RequestingUsername = template.Arguments.Single(arg => arg.Name.ToPlainText() == "username").Value.ToPlainText();
                jobRequest.Comment = template.Arguments.Single(arg => arg.Name.ToPlainText() == "comment").Value.ToPlainText();

                //WikiMedia timestamp example: 14:58, 30 June 2020-04:00:00
                var timeString = template.Arguments.Single(arg => arg.Name.ToPlainText() == "timestamp").Value.ToPlainText()[..^6];
                jobRequest.SubmittedDateUTC = DateTime.ParseExact($"{timeString}{timeZoneString}", "HH:mm, dd MMMM yyyyKKKK", new CultureInfo("en-US")).ToUniversalTime();
            }
            else
            {
                jobRequest.Status = JobStatus.ToBeProcessed;
                jobRequest.RequestingUsername = "Wikibot";
                jobRequest.Comment = "Internally scheduled job";
                jobRequest.SubmittedDateUTC = DateTime.UtcNow;
            }

            jobRequest.RawRequest = template.ToString();
            return jobRequest;

        }

        private static string GetTimeZoneString(TimeZoneInfo timezone)
        {
            var offset = timezone.GetUtcOffset(DateTime.Now);
            var prefix = offset < TimeSpan.Zero ? "\\-" : "";
            var timeZoneFormat = prefix + "hh\\:mm";
            return offset.ToString(timeZoneFormat);

        }
    }
}
