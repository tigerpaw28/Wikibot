using LinqToWiki.Generated;
using WikiClientLibrary.Client;
using WikiClientLibrary.Sites;

namespace Wikibot.Logic.Logic
{
    public interface IWikiAccessLogic
    {
        Wiki GetLoggedInWiki();
        WikiSite GetLoggedInWikiSite(WikiClient client);
    }
}