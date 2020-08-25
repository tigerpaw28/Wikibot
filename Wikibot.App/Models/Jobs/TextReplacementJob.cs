using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wikibot.App.Jobs;
using WikiClientLibrary.Sites;
using Wikibot.App.Extensions;
using WikiClientLibrary.Pages;
using MwParserFromScratch;
using WikiFunctions;
using System.IO;
using Microsoft.Extensions.Configuration;
using Wikibot.App.Logic;
using WikiClientLibrary.Client;

namespace Wikibot.App.Jobs
{
    public class TextReplacementJob: WikiJob
    {

        public string FromText { get; set; }
        public string ToText { get; set; }
        public List<Page> PageNames { get; set; }

        public TextReplacementJob()
        { }

        public TextReplacementJob(Serilog.ILogger log)
        {
            Log = log;
        }

        public override void Execute()
        {
            SetJobStart();

            try
            {
                using (var client = new WikiClient())
                {
                    var site = WikiAccessLogic.GetLoggedInWikiSite(WikiConfig, client);
                    var PageList = GetPageList(site);

                    int counter = 0;
                    string filename = "";
                    string diff = "";
                    string filePath = "";

                    foreach (WikiPage page in PageList)
                    {
                        Log.Information("Processing page {PageName}", page.Title);
                        filename = "Diff-" + page.Title + "-" + ID + "-" + counter + ".txt"; //Set filename for this page
                        page.RefreshAsync(PageQueryOptions.FetchContent | PageQueryOptions.ResolveRedirects).Wait(); //Load page content

                        var beforeContent = page.Content;
                        var afterContent = page.Content.Replace(FromText, ToText);

                        if (Status != JobStatus.Approved) //Create diffs for approval
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
                Status = JobStatus.Failed;
                Log.Error(ex, $"TextReplacementJob with ID: {ID} failed.");
            }
            finally
            {
                SetJobEnd();
                SaveJob();
            }
        }

        private IEnumerable<WikiPage> GetPageList(WikiSite site)
        {
            if (PageNames == null || PageNames.Count == 0)
            {
                Log.Information("Searching for relevant pages for job {JobID}", ID);
                //Search for relevant pages
                PageNames = site.Search(FromText, 0).Result.Select(x => new Page { ID = 0, Name = x.Title }).ToList(); //Search Main namespace by default
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
