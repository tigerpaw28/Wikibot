using LinqToWiki.Generated;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using Wikibot.Logic.UserRetrievers;
using WikiClientLibrary;
using WikiClientLibrary.Client;
using WikiClientLibrary.Sites;

namespace Wikibot.Tests
{
    public static class Utilities
    {
        const string CONFIGURATION_ROOT_PATH = "D:\\Wikibot\\Wikibot\\Wikibot.Tests\\";
        public static IConfigurationRoot GetIConfigurationRoot()
        {
            return new ConfigurationBuilder()
                .SetBasePath(CONFIGURATION_ROOT_PATH)
                .AddJsonFile("appsettings.json", optional: true)
                .AddUserSecrets("e3dfcccf-0cb3-423a-b302-e3e92e95c128")
                .AddEnvironmentVariables()
                .Build();
        }

        public static Wiki GetWiki(IConfiguration config)
        {
            var wikiLoginConfig = config.GetSection("WikiLogin");
            var username = wikiLoginConfig["Username"];
            var password = wikiLoginConfig["Password"];
            var wiki = new Wiki("WikiBot", "https://tfwiki.net", "/mediawiki/api.php");
            var result = wiki.login(username, password);

            if (result.result == loginresult.NeedToken)
                result = wiki.login(username, password, token: result.token);

            if (result.result != loginresult.Success)
                throw new Exception(result.result.ToString());


            return wiki;
        }

        public static WikiSite GetWikiSite(IConfiguration config)
        {
            var client = new WikiClient
            {
                ClientUserAgent = "WCLQuickStart/1.0 (your user name or contact information here)"
            };
            var WikiConfig = config.GetSection("WikiLogin");
            var username = WikiConfig["Username"];
            var password = WikiConfig["Password"];
            var url = WikiConfig["APIUrl"];
            // You can create multiple WikiSite instances on the same WikiClient to share the state.
            var site = new WikiSite(client, url);

            // Wait for initialization to complete.
            // Throws error if any.
            site.Initialization.Wait();
            try
            {
                site.LoginAsync(username, password).Wait();
            }
            catch (WikiClientException ex)
            {
                Console.WriteLine(ex.Message);
                // Add your exception handler for failed login attempt.
                throw;
            }
            return site;
        }

        public static Serilog.ILogger GetLogger(IConfiguration config)
        {
            return new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .CreateLogger();
        }

        public static IUserRetriever GetUserRetriever(IConfiguration config)
        {
            var wiki = GetWiki(config);
            return new TFWikiUserRetriever(wiki);
        }

    }
}
