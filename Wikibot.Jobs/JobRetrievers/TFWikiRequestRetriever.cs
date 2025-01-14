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
using Wikibot.Logic.Jobs;
using Wikibot.Logic.Logic;
using Wikibot.Logic.UserRetrievers;
using WikiClientLibrary.Client;
using WikiClientLibrary.Pages;

namespace Wikibot.Logic.JobRetrievers
{
    public class TFWikiRequestRetriever: IWikiRequestRetriever
    {
        private List<WikiJobRequest> _requestDefinitions;
 
        private IConfiguration _config;
        private string _timeZoneID;
        private ILogger _log;
        private IWikiAccessLogic _wikiAccessLogic;
        private RequestData _database;
        private string _wikiRequestPage;
        private string _botRequestTemplate;
        private INotificationService _notificationService;
        private IUserRetriever _userRetriever;

        public List<WikiJobRequest> RequestDefinitions
        {
            get
            {
                _log.Information("Getting job definitions");
                _requestDefinitions = GetNewJobDefinitions().Result;
                return _requestDefinitions;
            }
        }

        public TFWikiRequestRetriever(IConfiguration configuration, ILogger log, IDataAccess dataAccess, INotificationService notificationService, IUserRetriever userRetriever)
        {
            _config = configuration;
            _wikiRequestPage = configuration["WikiRequestPage"];
            _botRequestTemplate = configuration["BotRequestTemplate"];
            _timeZoneID = configuration["RequestTimezoneID"];
            _log = log;
            _wikiAccessLogic = new WikiAccessLogic(configuration, log);
            _database = new RequestData(dataAccess);
            _notificationService = notificationService;
            _userRetriever = userRetriever;
        }

        public async Task<List<WikiJobRequest>> GetNewJobDefinitions()
        {
            IEnumerable<WikiJobRequest> jobs = null;
            
            using (var client = new WikiClient
            {
                ClientUserAgent = "WCLQuickStart/1.0 (your user name or contact information here)"
            })
            {
                try
                {
                    _log.Information($"Logging into Wiki");
                    var site = _wikiAccessLogic.GetLoggedInWikiSite(client);
                    
                    var page = new WikiPage(site, _wikiRequestPage);
                    _log.Information($"Fetching content from job request page {page.Title}");
                    // Fetch content from the job request page so we can build jobs from it
                    await page.RefreshAsync(PageQueryOptions.FetchContent
                                            | PageQueryOptions.ResolveRedirects);
                    if ((page?.Content ?? "").Length > 1)
                    {                    
                        var revisions = await site.Revisions(page.Title, "", 5, new System.Threading.CancellationToken());

                        var ast = new WikitextParser().Parse(page?.Content);
                        var templates = ast.EnumDescendants().OfType<Template>();

                        _log.Information("Building jobs.");
                        jobs = templates.Select(template => WikiJobRequestFactory.GetWikiJobRequest((JobType)Enum.Parse(typeof(JobType), template.Arguments.Single(arg => arg.Name.ToPlainText() == "type").Value.ToPlainText().Replace(" ", "") + "Job"), GetTimeZone(), template));
                        foreach(WikiJobRequest request in jobs)
                        {
                            _log.Information($"Request {request.RawRequest} has status {request.Status}");
                        }

                        jobs = jobs.Where(t => DoesUserHaveEditInLast15Minutes(revisions, t.RequestingUsername, t.RawRequest));

                    }
                    await site.LogoutAsync();
                }
                catch (Exception ex)
                {
                    _log.Error(ex, "An error occurred while trying to fetch new requests: ");
                }
            }

            return jobs?.ToList();
        }

        public async void UpdateRequests(List<WikiJobRequest> requests)
        {
            using (var client = new WikiClient
            {
                ClientUserAgent = "WCLQuickStart/1.0 (your user name or contact information here)"
            })
            
            {
                try
                {
                    // You can create multiple WikiSite instances on the same WikiClient to share the state.
                    var site = _wikiAccessLogic.GetLoggedInWikiSite(client);

                    var page = new WikiPage(site, _wikiRequestPage);

                    _log.Information("Pulling requests from job request page for status update.");

                    // Fetch content of job request page so we can update it
                    await page.RefreshAsync(PageQueryOptions.FetchContent
                                            | PageQueryOptions.ResolveRedirects);

                    var parser = new WikitextParser();
                    var wikiText = parser.Parse(page.Content);

                    foreach (WikiJobRequest request in requests)
                    {
                        _log.Information($"Updating request ID: {request.ID} with raw {request.RawRequest}");
                        //Find corresponding template in the page content
                        var templates = wikiText.EnumDescendants().OfType<Template>();
                        var requestTemplates = templates.Where(template => template.Name.ToPlainText().Equals(_botRequestTemplate));
                        _log.Information($"{requestTemplates.ToList().Count} templates found for template {_botRequestTemplate}");
                        _log.Information($"Template id: {requestTemplates.First().Arguments.SingleOrDefault(arg => arg.Name.ToPlainText().Equals("id"))}");
                        var singletemplate = requestTemplates.First(template => template.EqualsJob(request));

                        if (singletemplate.Arguments.SingleOrDefault(arg => arg.Name.ToPlainText().Equals("status")) == null) //Status argument doesn't exist in the template
                        {
                            var templateArgument = new TemplateArgument { Name = parser.Parse("status"), Value = parser.Parse(request.Status.ToString()) };
                            singletemplate.Arguments.Add(templateArgument);
                        }
                        else //Status argument exists
                        {
                            singletemplate.Arguments.Single(arg => arg.Name.ToPlainText().Equals("status")).Value = parser.Parse(request.Status.ToString());
                        }

                        if (singletemplate.Arguments.SingleOrDefault(arg => arg.Name.ToPlainText().Equals("id")) == null) //ID argument doesn't exist in the template
                        {
                            var templateArgument = new TemplateArgument { Name = parser.Parse("id"), Value = parser.Parse(request.ID.ToString()) };
                            singletemplate.Arguments.Add(templateArgument);
                        }

                        request.RawRequest = singletemplate.ToString();
                        _database.UpdateRaw(request.ID, request.RawRequest); //TODO: Make batch operation
                    }

                    //Update the content of the page object and push it live
                    await UpdatePageContent(wikiText.ToString(), "Updating request ids and statuses", page);
                    

                    // We're done here
                    await site.LogoutAsync();
                }
                catch (Exception ex)
                {
                    _log.Error(ex, "An error occurred while trying to update requests: ");
                }
            }
        }

        public async Task UpdatePageContent(string content, string message, WikiPage page)
        {
            page.Content = content;
            await page.UpdateContentAsync(message);
        }

        public WikiJob GetJobForRequest(WikiJobRequest request)
        {
            return WikiJobFactory.GetWikiJob(request, _log, _wikiAccessLogic, _config, _notificationService, _database, this, _userRetriever);
        } 

        private TimeZoneInfo GetTimeZone()
        {
            return TimeZoneInfo.FindSystemTimeZoneById(_timeZoneID);
        }

        private bool DoesUserHaveEditInLast15Minutes(IList<Revision> revisions, string user, string content)
        {
            if(revisions.Any(r=> r.UserName == user && r.TimeStamp.ToLocalTime() >= DateTime.Now.AddMinutes(-15)))
            {
                var latestRevision = revisions.Where(r => r.UserName == user
                && r.TimeStamp.ToLocalTime() >= DateTime.Now.AddMinutes(-15)).OrderBy(r => r.TimeStamp).Last();
                return latestRevision.Content.Contains(content);
            }
            return false;
        }
    }
}
