﻿using LinqToWiki.Generated;
using Microsoft.Extensions.Configuration;
using WikiClientLibrary.Client;
using WikiClientLibrary.Sites;

namespace Wikibot.Logic.Logic
{
    public interface IWikiAccessLogic
    {
        Wiki GetLoggedInWiki(IConfigurationSection wikiConfig);
        WikiSite GetLoggedInWikiSite(IConfigurationSection wikiConfig, WikiClient client);
    }
}