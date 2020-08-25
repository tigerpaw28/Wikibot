using LinqToWiki.Generated;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WikiClientLibrary;
using WikiClientLibrary.Client;
using WikiClientLibrary.Sites;

namespace Wikibot.App.Logic
{
    public static class WikiAccessLogic
    {

        public static Wiki GetLoggedInWiki(IConfigurationSection wikiConfig)
        {
            var url = wikiConfig["APIUrl"];
            var path = wikiConfig["APIPath"];
            Wiki wiki = new Wiki("WikiBot", url, "/mediawiki/api.php");

            LogIntoWiki(wiki, wikiConfig);

            return wiki;
        }

        public static WikiSite GetLoggedInWikiSite(IConfigurationSection wikiConfig, WikiClient client)
        {
            var url = wikiConfig["APIUrl"];
            var site = new WikiSite(client, url);
            LogIntoWikiSite(site, wikiConfig);
            return site;
        }

        private static void LogIntoWikiSite(WikiSite site, IConfigurationSection wikiConfig)
        {
            var username = wikiConfig["Username"];
            var password = wikiConfig["Password"];

            // Wait for initialization to complete.
            // Throws error if any.
            site.Initialization.Wait();

            site.LoginAsync(username, password).Wait();
        }

        private static void LogIntoWiki(Wiki wiki, IConfigurationSection wikiConfig)
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
