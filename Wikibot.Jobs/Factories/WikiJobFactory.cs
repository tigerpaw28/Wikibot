using Microsoft.Extensions.Configuration;
using System;
using Wikibot.Logic.Jobs;
using Wikibot.Logic.Logic;
using Wikibot.DataAccess;
using Wikibot.DataAccess.Objects;
using Wikibot.Logic.JobRetrievers;

namespace Wikibot.Logic.Factories
{
    public static class WikiJobFactory
    {
        public static WikiJob GetWikiJob(WikiJobRequest request, Serilog.ILogger log, IWikiAccessLogic wikiAccessLogic, IConfiguration config, RequestData jobData, IWikiJobRetriever retriever)
        {

            WikiJob job;

            switch (request.JobType)
            {
                case JobType.TextReplacementJob:
                    var throttleSpeedInSeconds = int.Parse(config["ThreadThrottleSpeedInSeconds"]);
                    job = new TextReplacementJob(log, wikiAccessLogic, retriever, jobData, throttleSpeedInSeconds);
                    break;
                case JobType.LinkFixJob:
                    throttleSpeedInSeconds = int.Parse(config["ThreadThrottleSpeedInSeconds"]);
                    job = new LinkFixJob(log, wikiAccessLogic, retriever, jobData, throttleSpeedInSeconds);
                    break;
                default:
                    throw new Exception("Job type is undefined");
            }

            job.Configuration = config;
            job.JobData = jobData;
            job.Request = request;
            job.Log = log;

            return job;

        }
    }
}
