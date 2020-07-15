using LinqToWiki.Generated;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WikiClientLibrary;
using WikiClientLibrary.Client;
using WikiClientLibrary.Sites;

namespace Wikibot.App.Jobs
{
    public class AbstractJob : WikiJob
    {
        public IConfiguration Configuration;

        public IConfigurationSection WikiConfig
        {
            get
            {
                return Configuration.GetSection("WikiLogin");
            }
        }
        private Wiki _wiki;
        public Wiki Wiki
        {
            get
            {
                if (_wiki == null)
                {
                    var wikiLoginConfig = Configuration.GetSection("WikiLogin");
                    var username = WikiConfig["Username"];
                    var password = WikiConfig["Password"];
                    _wiki = new Wiki("WikiBot", "https://tfwiki.net", "/mediawiki/api.php");
                    var result = _wiki.login(username, password);

                    if (result.result == loginresult.NeedToken)
                        result = _wiki.login(username, password, token: result.token);

                    if (result.result == loginresult.Success)
                        return _wiki;
                    else
                        throw new Exception(result.result.ToString());
                }
                else
                    return _wiki;
            }
        }

        private WikiClient _client;
        public WikiClient Client
        {
            get
            {
                if (_client == null)
                {
                    var client = new WikiClient
                    {
                        ClientUserAgent = "WCLQuickStart/1.0 (your user name or contact information here)"
                    };
                    _client = client;
                }
                return _client;
            }
        }
        private WikiSite _site;
        public WikiSite Site
        {
            get {
                if (_site == null)
                {
                    var username = WikiConfig["Username"];
                    var password = WikiConfig["Password"];
                    var url = WikiConfig["APIUrl"];
                    // You can create multiple WikiSite instances on the same WikiClient to share the state.
                    _site = new WikiSite(Client, url);

                    // Wait for initialization to complete.
                    // Throws error if any.
                    _site.Initialization.Wait();
                    try
                    {
                        _site.LoginAsync(username, password).Wait();
                    }
                    catch (WikiClientException ex)
                    {
                        Console.WriteLine(ex.Message);
                        // Add your exception handler for failed login attempt.
                        throw;
                    }
                }
                return _site;
            }
        }
    }
}
