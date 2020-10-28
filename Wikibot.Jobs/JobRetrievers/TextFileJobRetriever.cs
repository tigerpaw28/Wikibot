using Microsoft.Extensions.Configuration;
using MwParserFromScratch;
using MwParserFromScratch.Nodes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Wikibot.DataAccess;
using Wikibot.DataAccess.Objects;
using Wikibot.Logic.Extensions;
using Wikibot.Logic.Factories;

namespace Wikibot.Logic.JobRetrievers
{
    public class TextFileJobRetriever : IWikiJobRetriever
    {
        private List<WikiJobRequest> _jobDefinitions;
        private string _pathToTextFile;
        private readonly IConfiguration _config;

        public List<WikiJobRequest> JobDefinitions 
        {
            get {
                if (_jobDefinitions == null)
                    _jobDefinitions = GetNewJobDefinitions().Result;
                return _jobDefinitions;
            }
        }
        
        public TextFileJobRetriever(IConfiguration configuration, string pathToTextFile)
        {
            _pathToTextFile = pathToTextFile;
            _config = configuration;
        }

        public async Task<List<WikiJobRequest>> GetNewJobDefinitions()
        {
            var ast = await parseFile();
            var templates = ast.Lines.SelectMany(x=> x.EnumDescendants().OfType<Template>());
            var jobs = templates.Select(template => WikiJobRequestFactory.GetWikiJobRequest(JobType.TextReplacementJob, GetTimeZone(), template));
            return jobs.ToList();
        }

        public void UpdateRequests(List<WikiJobRequest> jobs)
        {
            var wikiText = parseFile().Result;
            foreach(WikiJobRequest job in jobs)
            {
                var templates = wikiText.Lines.SelectMany(x => x.EnumDescendants().OfType<Template>());
                var singletemplate = templates.First(x => x.Name.ToPlainText().Equals("deceptitran") && x.EqualsJob(job));
                singletemplate.Arguments.Single(arg=> arg.Name.ToPlainText().Equals("status")).Value = new WikitextParser().Parse(job.Status.ToString());
            }
            File.WriteAllText(_pathToTextFile, wikiText.ToString());
        }

        private async Task<Wikitext> parseFile()
        { 
           string contents = await File.ReadAllTextAsync(_pathToTextFile);
           return new WikitextParser().Parse(contents);
        }

        private TimeZoneInfo GetTimeZone()
        {
            return TimeZoneInfo.FindSystemTimeZoneById(_config["RequestTimezoneID"]);
        }

    }
}