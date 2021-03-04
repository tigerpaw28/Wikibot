using LinqToWiki.Generated;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using WikiClientLibrary.Client;
using WikiClientLibrary.Sites;

namespace Wikibot.Logic.Logic
{
    public class WikiAccessLogic : IWikiAccessLogic
    {

        public Wiki GetLoggedInWiki(IConfigurationSection wikiConfig)
        {
            var url = wikiConfig["APIUrl"];
            var path = wikiConfig["APIPath"];
            Wiki wiki = new Wiki("WikiBot", url, path);

            LogIntoWiki(wiki, wikiConfig);

            return wiki;
        }

        public WikiSite GetLoggedInWikiSite(IConfigurationSection wikiConfig, WikiClient client, ILogger log)
        {
            var url = wikiConfig["APIUrl"];
            var path = wikiConfig["APIPath"];
            var uriBuilder = new UriBuilder(url);
            uriBuilder.Path = path;
            var site = new WikiSite(client, uriBuilder.Uri.ToString());
            LogIntoWikiSite(site, wikiConfig, log);
            return site;
        }

        private void LogIntoWikiSite(WikiSite site, IConfigurationSection wikiConfig, ILogger log)
        {
            var username = wikiConfig["Username"];
            var password = wikiConfig["Password"];

            // Wait for initialization to complete.
            // Throws error if any.
            site.Initialization.Wait();
            try
            {
                site.LoginAsync(username, password).Wait();
            }
            catch(Exception ex)
            {
                log.Error(ex, "Error logging in:");
                throw;
            }
        }

        private void LogIntoWiki(Wiki wiki, IConfigurationSection wikiConfig)
        {
            var username = wikiConfig["Username"];
            var password = wikiConfig["Password"];

            var result = wiki.login(username, password);

            if (result.result == loginresult.NeedToken)
                result = wiki.login(username, password, token: result.token);


            if (result.result != loginresult.Success)
            {
                throw new Exception(result.result.ToString());
            }
        }
    }
}
