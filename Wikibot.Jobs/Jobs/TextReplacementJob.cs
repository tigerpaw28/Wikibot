﻿using MwParserFromScratch;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Wikibot.DataAccess;
using Wikibot.DataAccess.Objects;
using Wikibot.Logic.Extensions;
using Wikibot.Logic.Logic;
using WikiClientLibrary.Client;
using WikiClientLibrary.Pages;
using WikiClientLibrary.Sites;
using WikiFunctions;

namespace Wikibot.Logic.Jobs
{
    public class TextReplacementJob: WikiJob
    {

        public List<Page> PageNames { get; set; }

        private IWikiAccessLogic _wikiAccessLogic;

        public TextReplacementJob()
        { }

        public TextReplacementJob(Serilog.ILogger log, IWikiAccessLogic wikiAccessLogic)
        {
            Log = log;
            _wikiAccessLogic = wikiAccessLogic;

        }

        public override void Execute()
        {
            SetJobStart();

            try
            {
                using (var client = new WikiClient())
                {
                    var site = _wikiAccessLogic.GetLoggedInWikiSite(WikiConfig, client);
                    var PageList = GetPageList(site);

                    int counter = 0;
                    string filename = "";
                    string diff = "";
                    string filePath = "";

                    foreach (WikiPage page in PageList)
                    {
                        Log.Information("Processing page {PageName}", page.Title);
                        filename = "Diff-" + page.Title + "-" + Request.ID + "-" + counter + ".txt"; //Set filename for this page
                        page.RefreshAsync(PageQueryOptions.FetchContent | PageQueryOptions.ResolveRedirects).Wait(); //Load page content

                        var beforeContent = page.Content;
                        var afterContent = page.Content.Replace(FromText, ToText);

                        if (Request.Status != JobStatus.Approved) //Create diffs for approval
                        {
                            Log.Information("Generating diff for page {PageName}", page.Title);
                            diff = new WikiDiff().GetDiff(beforeContent, afterContent, 1);
                            filePath = Path.Combine(Configuration["DiffDirectory"], filename);
                            File.WriteAllText(filePath, diff);
                        }
                        else //Apply changes
                        {
                            Log.Information("Applying replacement for page {PageName}", page.Title);
                            var editMessage = $"Wikibot Text Replacement {FromText} => {ToText}";
                            UpdatePageContentWithMessage(page, afterContent, editMessage);
                        }

                    }
                }
            }
            catch(Exception ex)
            {
                Request.Status = JobStatus.Failed;
                Log.Error(ex, $"TextReplacementJob with ID: {Request.ID} failed.");
            }
            finally
            {
                SetJobEnd();
                SaveRequest();
            }
        }

        private IEnumerable<WikiPage> GetPageList(WikiSite site)
        {
            if (PageNames == null || PageNames.Count == 0)
            {
                Log.Information("Searching for relevant pages for job {JobID}", Request.ID);
                //Search for relevant pages
                PageNames = site.Search(FromText, 0).Result.Select(x => new Page { PageID = 0, Name = x.Title }).ToList(); //Search Main namespace by default
            }
            return PageNames.Select(page => new WikiPage(site, page.Name));
        }

        private void UpdatePageContentWithMessage(WikiPage page, string content, string editMessage)
        {
            var parser = new WikitextParser();
            var wikiText = parser.Parse(content);
            page.Content = wikiText.ToString();
            page.UpdateContentAsync(editMessage).Wait();
        }
    }
}