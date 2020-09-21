﻿using Microsoft.Extensions.Configuration;
using MwParserFromScratch;
using MwParserFromScratch.Nodes;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wikibot.DataAccess;
using Wikibot.DataAccess.Objects;
using Wikibot.Logic.Extensions;
using Wikibot.Logic.Factories;
using Wikibot.Logic.Logic;
using WikiClientLibrary.Client;
using WikiClientLibrary.Pages;

namespace Wikibot.Logic.JobRetrievers
{
    public class TFWikiJobRetriever: IWikiJobRetriever
    {
        private List<WikiJobRequest> _jobDefinitions;
        private IConfigurationSection _wikiLoginConfig;
        private string _timeZoneID;
        private ILogger _log;
        private IWikiAccessLogic _wikiAccessLogic;
        public List<WikiJobRequest> JobDefinitions
        {
            get
            {
                if (_jobDefinitions == null)
                    _jobDefinitions = GetNewJobDefinitions().Result;
                return _jobDefinitions;
            }
        }

        public TFWikiJobRetriever(IConfiguration configuration, ILogger log, IWikiAccessLogic wikiAccessLogic)
        {
            _wikiLoginConfig = configuration.GetSection("WikiLogin");
            _timeZoneID = configuration["RequestTimezoneID"];
            _log = log;
            _wikiAccessLogic = wikiAccessLogic;
        }

        public async Task<List<WikiJobRequest>> GetNewJobDefinitions()
        {
            IEnumerable<WikiJobRequest> jobs;
            
            using (var client = new WikiClient
            {
                ClientUserAgent = "WCLQuickStart/1.0 (your user name or contact information here)"
            })
            {
                var site = _wikiAccessLogic.GetLoggedInWikiSite(_wikiLoginConfig, client); //new WikiSite(client, _wikiLoginConfig["APIUrl"]);
                var page = new WikiPage(site, "User:Tigerpaw28/Sandbox/WikibotRequests");

                _log.Information("Fetching content from job request page.");
                // Fetch content from the job request page so we can build jobs from it
                await page.RefreshAsync(PageQueryOptions.FetchContent
                                        | PageQueryOptions.ResolveRedirects);

                //var text = "Paragraph.\n* Item1\n* Item2\n";
                //var revision = page.CreateRevisionsGenerator().EnumItemsAsync().LastAsync().Result;
                //var fullRev = await Revision.FetchRevisionAsync(site, revision.Id);
                //var content = fullRev.Content;
                var ast = new WikitextParser().Parse(page.Content);
                var templates = ast.Lines.First().EnumDescendants().OfType<Template>();

                _log.Information("Building jobs.");
                jobs = templates.Select(template => WikiJobRequestFactory.GetWikiJobRequest((JobType)Enum.Parse(typeof(JobType), template.Arguments.Single(arg => arg.Name.ToPlainText() == "type").Value.ToPlainText().Replace(" ", "") + "Job"), GetTimeZone(), template));
            }
            return jobs.ToList();
        }

        public async void MarkJobStatuses(List<WikiJobRequest> requests)
        {
            using (var client = new WikiClient
            {
                ClientUserAgent = "WCLQuickStart/1.0 (your user name or contact information here)"
            })
            {
                // You can create multiple WikiSite instances on the same WikiClient to share the state.
                var site = _wikiAccessLogic.GetLoggedInWikiSite(_wikiLoginConfig, client);

                var page = new WikiPage(site, "User:Tigerpaw28/Sandbox/WikibotRequests");

                _log.Information("Fetching content from job request page.");

                // Fetch content of job request page so we can update it
                await page.RefreshAsync(PageQueryOptions.FetchContent
                                        | PageQueryOptions.ResolveRedirects);

                var parser = new WikitextParser();
                var wikiText = parser.Parse(page.Content);

                foreach (WikiJobRequest request in requests)
                {
                    //Find corresponding template in the page content
                    var templates = wikiText.Lines.SelectMany(x => x.EnumDescendants().OfType<Template>());
                    var singletemplate = templates.First(x => x.Name.ToPlainText().Equals("User:Tigerpaw28/Sandbox/Template:WikiBotRequest") && x.EqualsJob(request));

                    if (singletemplate.Arguments.SingleOrDefault(arg => arg.Name.ToPlainText().Equals("status")) == null) //Status argument doesn't exist in the template
                    {
                        var templateArgument = new TemplateArgument { Name = parser.Parse("status"), Value = parser.Parse(request.Status.ToString()) };
                        singletemplate.Arguments.Add(templateArgument);
                    }
                    else //Status argument exists
                    {
                        singletemplate.Arguments.Single(arg => arg.Name.ToPlainText().Equals("status")).Value = parser.Parse(request.Status.ToString());
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