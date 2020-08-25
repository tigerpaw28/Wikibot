using LinqToWiki;
using LinqToWiki.Generated;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WikiClientLibrary.Client;
using WikiClientLibrary.Pages;
using WikiClientLibrary.Sites;
using MwParserFromScratch;
using MwParserFromScratch.Nodes;
using Wikibot.App.Jobs;
using WikiClientLibrary;
using WikiClientLibrary.Generators;
using System.Security.Cryptography.X509Certificates;
using Wikibot.App.Extensions;
using Microsoft.Extensions.Configuration;
using Serilog;
using Wikibot.App.Logic;

namespace Wikibot.App.JobRetrievers
{
    public class TFWikiJobRetriever: IWikiJobRetriever
    {
        private List<WikiJob> _jobDefinitions;
        private IConfigurationSection _wikiLoginConfig;
        private string _timeZoneID;
        private WikiSite _site;
        private ILogger _log;
        public List<WikiJob> JobDefinitions
        {
            get
            {
                if (_jobDefinitions == null)
                    _jobDefinitions = GetNewJobDefinitions().Result;
                return _jobDefinitions;
            }
        }

        public TFWikiJobRetriever(IConfiguration configuration, Serilog.ILogger log)
        {
            _wikiLoginConfig = configuration.GetSection("WikiLogin");
            _timeZoneID = configuration["RequestTimezoneID"];
            _log = log;
        }

        public async Task<List<WikiJob>> GetNewJobDefinitions()
        {
            IEnumerable<WikiJob> jobs;
            
            using (var client = new WikiClient
            {
                ClientUserAgent = "WCLQuickStart/1.0 (your user name or contact information here)"
            })
            {
                var site = WikiAccessLogic.GetLoggedInWikiSite(_wikiLoginConfig, client); //new WikiSite(client, _wikiLoginConfig["APIUrl"]);
                var page = new WikiPage(site, "User:Tigerpaw28/Sandbox/WikibotRequests");

                _log.Information("Fetching content from job request page.");
                // Fetch content from the job request page so we can build jobs from it
                await page.RefreshAsync(PageQueryOptions.FetchContent
                                        | PageQueryOptions.ResolveRedirects);

                var parser = new WikitextParser();
                //var text = "Paragraph.\n* Item1\n* Item2\n";
                //var revision = page.CreateRevisionsGenerator().EnumItemsAsync().LastAsync().Result;
                //var fullRev = await Revision.FetchRevisionAsync(site, revision.Id);
                //var content = fullRev.Content;
                var ast = parser.Parse(page.Content);
                var templates = ast.Lines.First<LineNode>().EnumDescendants().OfType<Template>();

                _log.Information("Building jobs.");
                var jobFactory = new WikiJobFactory();
                jobs = templates.Select(template => jobFactory.GetWikiJob((JobType)Enum.Parse(typeof(JobType), template.Arguments.Single(arg => arg.Name.ToPlainText() == "type").Value.ToPlainText().Replace(" ", "") + "Job"), GetTimeZone(), _log, template));
            }
            return jobs.ToList();
        }

        public async void MarkJobStatuses(List<WikiJob> jobs)
        {
            using (var client = new WikiClient
            {
                ClientUserAgent = "WCLQuickStart/1.0 (your user name or contact information here)"
            })
            {
                // You can create multiple WikiSite instances on the same WikiClient to share the state.
                var site = WikiAccessLogic.GetLoggedInWikiSite(_wikiLoginConfig, client);  //new WikiSite(client, _wikiLoginConfig["APIUrl"]);

                var page = new WikiPage(site, "User:Tigerpaw28/Sandbox/WikibotRequests");

                _log.Information("Fetching content from job request page.");
                // Fetch content of job request page so we can update it
                await page.RefreshAsync(PageQueryOptions.FetchContent
                                        | PageQueryOptions.ResolveRedirects);

                var parser = new WikitextParser();
                var wikiText = parser.Parse(page.Content);

                foreach (WikiJob job in jobs)
                {
                    //Find corresponding template in the page content
                    var templates = wikiText.Lines.SelectMany(x => x.EnumDescendants().OfType<Template>());
                    var singletemplate = templates.First(x => x.Name.ToPlainText().Equals("User:Tigerpaw28/Sandbox/Template:WikiBotRequest") && x.EqualsJob(job));

                    if (singletemplate.Arguments.SingleOrDefault(arg => arg.Name.ToPlainText().Equals("status")) == null) //Status argument doesn't exist in the template
                    {
                        var templateArgument = new TemplateArgument();
                        templateArgument.Name = parser.Parse("status");
                        templateArgument.Value = parser.Parse(job.Status.ToString());
                        singletemplate.Arguments.Add(templateArgument);
                    }
                    else //Status argument exists
                    {
                        singletemplate.Arguments.Single(arg => arg.Name.ToPlainText().Equals("status")).Value = new WikitextParser().Parse(job.Status.ToString());
                    }


                }

                await UpdatePageContent(wikiText.ToString(), "Test page update", page);
                //Update the content of the page object and push it live


                // We're done here
                await site.LogoutAsync();
            }
        }

        private async Task UpdatePageContent(string content, string message, WikiPage page)
        {
            page.Content = content;
            await page.UpdateContentAsync(message);
        }

        private TimeZoneInfo GetTimeZone()
        {
            return TimeZoneInfo.FindSystemTimeZoneById(_timeZoneID);
        }
    }
}
