using LinqToWiki.Generated;
using Microsoft.Extensions.Configuration;
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
            Wiki wiki = new Wiki("WikiBot", url, "/mediawiki/api.php");

            LogIntoWiki(wiki, wikiConfig);

            return wiki;
        }

        public WikiSite GetLoggedInWikiSite(IConfigurationSection wikiConfig, WikiClient client)
        {
            var url = wikiConfig["APIUrl"];
            var site = new WikiSite(client, url);
            LogIntoWikiSite(site, wikiConfig);
            return site;
        }

        private void LogIntoWikiSite(WikiSite site, IConfigurationSection wikiConfig)
        {
            var username = wikiConfig["Username"];
            var password = wikiConfig["Password"];

            // Wait for initialization to complete.
            // Throws error if any.
            site.Initialization.Wait();

            site.LoginAsync(username, password).Wait();
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
