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
using Wikibot.App.Models.Jobs;
using System.Security.Cryptography.X509Certificates;
using Wikibot.App.Extensions;
using Microsoft.Extensions.Configuration;

namespace Wikibot.App.JobRetrievers
{
    public class TFWikiJobRetriever:IWikiJobRetriever
    {
        private List<WikiJob> _jobDefinitions;
        private IConfiguration _config;
        private WikiSite _site;
        public List<WikiJob> JobDefinitions
        {
            get
            {
                if (_jobDefinitions == null)
                    _jobDefinitions = GetNewJobDefinitions().Result;
                return _jobDefinitions;
            }
        }

        public TFWikiJobRetriever(IConfiguration configuration, WikiSite site)
        {
            _config = configuration;
            _site = site;
        }

        public async Task<List<WikiJob>> GetNewJobDefinitions()
        {
            IEnumerable<WikiJob> jobs;

            var page = new WikiPage(_site, "User:Tigerpaw28/Sandbox/WikibotRequests");

            // Fetch basic information and content of 1 page from server
            await page.RefreshAsync(PageQueryOptions.FetchContent
                                    | PageQueryOptions.ResolveRedirects );

            var parser = new WikitextParser();
            //var text = "Paragraph.\n* Item1\n* Item2\n";
            //var revision = page.CreateRevisionsGenerator().EnumItemsAsync().LastAsync().Result;
            //var fullRev = await Revision.FetchRevisionAsync(site, revision.Id);
            //var content = fullRev.Content;
            var ast = parser.Parse(page.Content);
            var templates = ast.Lines.First<LineNode>().EnumDescendants().OfType<Template>();
            var jobFactory = new WikiJobFactory();
            jobs = templates.Select(template => jobFactory.GetWikiJob((JobType)Enum.Parse(typeof(JobType),template.Arguments.Single(arg => arg.Name.ToPlainText() == "type").Value.ToPlainText().Replace(" ","")+"Job"), template));
                
            Console.WriteLine(templates.First().ToString());

            return jobs.ToList();
        }

        public async void MarkJobStatuses(List<WikiJob> jobs)
        {
            using (var client = new WikiClient
            {
                ClientUserAgent = "WCLQuickStart/1.0 (your user name or contact information here)"
            })
            {
                var wikiLoginConfig = _config.GetSection("WikiLogin");
                // You can create multiple WikiSite instances on the same WikiClient to share the state.
                var site = new WikiSite(client, wikiLoginConfig["APIUrl"]);

                var username = wikiLoginConfig["Username"];
                var password = wikiLoginConfig["Password"];

                // Wait for initialization to complete.
                // Throws error if any.
                await site.Initialization;
                try
                {
                    await site.LoginAsync(username, password);
                }
                catch (WikiClientException ex)
                {
                    Console.WriteLine(ex.Message);
                    // Add your exception handler for failed login attempt.
                }
                var page = new WikiPage(site, "User:Tigerpaw28/Sandbox/WikibotRequests");
                // Fetch basic information and content of 1 page from server
                await page.RefreshAsync(PageQueryOptions.FetchContent
                                        | PageQueryOptions.ResolveRedirects);

                var parser = new WikitextParser();
                var wikiText = parser.Parse(page.Content);

                foreach (WikiJob job in jobs)
                {
                    var templates = wikiText.Lines.SelectMany(x => x.EnumDescendants().OfType<Template>());
                    var singletemplate = templates.First(x => x.Name.ToPlainText().Equals("User:Tigerpaw28/Sandbox/Template:WikiBotRequest") && x.EqualsJob(job));
                    if (singletemplate.Arguments.SingleOrDefault(arg => arg.Name.ToPlainText().Equals("status")) == null)
                    {
                        var templateArgument = new TemplateArgument();
                        templateArgument.Name = parser.Parse("status");
                        templateArgument.Value = parser.Parse(job.Status.ToString());
                        singletemplate.Arguments.Add(templateArgument);
                    }
                    else
                    {
                        singletemplate.Arguments.Single(arg => arg.Name.ToPlainText().Equals("status")).Value = new WikitextParser().Parse(job.Status.ToString());
                    }


                }
                page.Content = wikiText.ToString();
                await page.UpdateContentAsync("Test page update");
                // We're done here
                await site.LogoutAsync();
            }
            //File.WriteAllText(_pathToTextFile, wikiText.ToString());
        }
    }
}
