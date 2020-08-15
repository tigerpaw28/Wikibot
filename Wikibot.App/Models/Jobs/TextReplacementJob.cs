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

namespace Wikibot.App.Jobs
{
    public class TextReplacementJob: AbstractJob
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
                if (PageNames == null || PageNames.Count == 0)
                {
                    Log.Information("Searching for relevant pages for job {JobID}", ID);
                    //Search for relevant pages
                    PageNames = Site.Search(FromText, 0).Result.Select(x=> new Page { ID = 0, Name = x.Title }).ToList(); //Search Main namespace by default
                }
                var PageList = PageNames.Select(page=> new WikiPage(Site, page.Name));

                
                int counter = 0;
                string filename = "";
                string fileContent = "";

                foreach(WikiPage page in PageList)
                {
                    Log.Information("Processing page {PageName}", page.Title);
                    filename = "Diff-" + page.Title + "-" + this.ID + "-" + counter + ".txt"; //Set filename for this page
                    page.RefreshAsync(PageQueryOptions.FetchContent | PageQueryOptions.ResolveRedirects).Wait(); //Load page content
                        
                    var beforeContent = page.Content;
                    var afterContent = page.Content.Replace(FromText, ToText);
                    
                    if (Status != JobStatus.Approved) //Create diffs for approval
                    {
                        Log.Information("Generating diff for page {PageName}", page.Title);
                        fileContent = new WikiDiff().GetDiff(beforeContent, afterContent, 1);
                        var filePath = Path.Combine(Configuration["DiffDirectory"], filename);
                        File.WriteAllText(filePath, fileContent);
                    }
                    else //Apply changes
                    {
                        Log.Information("Applying replacement for page {PageName}", page.Title);
                        var parser = new WikitextParser();
                        var wikiText = parser.Parse(afterContent);
                        page.Content = wikiText.ToString();
                        var editMessage = String.Format("Wikibot Text Replacement {0} => {1}", FromText, ToText);
                        page.UpdateContentAsync(editMessage).Wait();
                    }
                }
            }
            catch(Exception ex)
            {
                Status = JobStatus.Failed;
                Log.Error(ex, $"TextReplacementJob {this.ID} failed.");
            }
            finally
            {
                CleanUp();
                SetJobEnd();
                SaveJob();
            }
        }
    }
}
