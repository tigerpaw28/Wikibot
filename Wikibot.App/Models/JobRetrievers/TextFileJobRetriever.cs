using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using LinqToWiki;
using LinqToWiki.Generated;
using MwParserFromScratch;
using MwParserFromScratch.Nodes;
using System.Linq;
using Wikibot.App.Jobs;
using Wikibot.App.Models.Jobs;
using Wikibot.App.Extensions;
using Microsoft.Extensions.Configuration;

namespace Wikibot.App.JobRetrievers
{
    public class TextFileJobRetriever : IWikiJobRetriever
    {
        private List<WikiJob> _jobDefinitions;
        private JobContext _context;
        private string _pathToTextFile;
        private IConfiguration _config;

        public List<WikiJob> JobDefinitions 
        {
            get {
                if (_jobDefinitions == null)
                    _jobDefinitions = GetNewJobDefinitions().Result;
                return _jobDefinitions;
            }
        }
        
        public TextFileJobRetriever(JobContext context, IConfiguration config, string pathToTextFile)
        {
            _context = context;
            _pathToTextFile = pathToTextFile;
            _config = config;
        }

        public void GetDefinitions(string pathToTextFile, Wiki wiki)
        {
            Task<string> task = File.ReadAllTextAsync(pathToTextFile);
            string contents = task.Result;
            Regex regex = new Regex("^{{.*}}$");
            throw new NotImplementedException();
        }

        public async Task<List<WikiJob>> GetNewJobDefinitions()
        {
            var ast = parseFile().Result;
            var templates = ast.Lines.SelectMany(x=> x.EnumDescendants().OfType<Template>());
            var jobFactory = new WikiJobFactory(_context, _config);
            var jobs = templates.Select(template => jobFactory.GetWikiJob(JobType.TextReplacementJob, template));
            return jobs.ToList();
        }

        public void MarkJobStatuses(List<WikiJob> jobs)
        {
            var wikiText = parseFile().Result;
            foreach(WikiJob job in jobs)
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
            var parser = new WikitextParser();
           return parser.Parse(contents);
        }

    }
}