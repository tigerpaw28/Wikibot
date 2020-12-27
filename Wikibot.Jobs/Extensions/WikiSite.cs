using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WikiClientLibrary;
using WikiClientLibrary.Client;
using WikiClientLibrary.Sites;

namespace Wikibot.Logic.Extensions
{
    public static class WikiSiteExtension
    {
        const int MaxCount = 50;

        /// <summary>
        /// Performs an opensearch and get results, often used for search box suggestions.
        /// (MediaWiki 1.25 or OpenSearch extension)
        /// </summary>
        /// <param name="searchExpression">The beginning part of the title to be searched.</param>
        /// <param name="defaultNamespaceId">Default namespace id to search. See <see cref="BuiltInNamespaces"/> for a list of possible namespace ids.</param>
        /// <returns>Search result.</returns>
        public static async Task<IList<SearchResultEntry>> Search(this WikiSite site, string searchExpression, int defaultNamespaceId)
        {
            return await Search(site, searchExpression, MaxCount, defaultNamespaceId, SearchOptions.text, CancellationToken.None);
        }
        /// <summary>
        /// Performs an opensearch and get results, often used for search box suggestions.
        /// (MediaWiki 1.25 or OpenSearch extension)
        /// </summary>
        /// <param name="searchExpression">The beginning part of the title to be searched.</param>
        /// <param name="maxCount">Maximum number of results to return. No more than 500 (5000 for bots) allowed.</param>
        /// <param name="defaultNamespaceId">Default namespace id to search. See <see cref="BuiltInNamespaces"/> for a list of possible namespace ids.</param>
        /// <param name="options">Other options.</param>
        /// <param name="cancellationToken">The cancellation token that will be checked prior to completing the returned task.</param>
        /// <returns>Search result.</returns>
        public static async Task<IList<SearchResultEntry>> Search(this WikiSite site, string searchExpression, int maxCount,
            int defaultNamespaceId, SearchOptions options, CancellationToken cancellationToken)
        {
            int offset = 0;
            var results = new JArray();
            while (offset >= 0)
            {
                var jresult = await site.InvokeMediaWikiApiAsync(new MediaWikiFormRequestMessage(new
                {
                    action = "query",
                    list = "search",
                    srnamespace = defaultNamespaceId,
                    srsearch = searchExpression,
                    srlimit = maxCount,
                    srwhat = options.ToString(),
                    sroffset = offset
                }), cancellationToken);
                
                var query = jresult["query"];
                results.Merge((JArray)query["search"]);
                var querycontinue = jresult["query-continue"];
                if(querycontinue != null)
                {
                    offset = Int32.Parse(querycontinue["search"]["sroffset"].ToString());
                }
                else
                {
                    offset = -1;
                }
            }
            var result = new List<SearchResultEntry>();
            foreach(JToken token in results)
            {
                var entry = new SearchResultEntry();
                entry.Title = token["title"].ToString();
                entry.NamespaceID = int.Parse(token["ns"].ToString());
                result.Add(entry);
            }

            return result;
        }

        public static async Task<IList<SearchResultEntry>> BackLinks(this WikiSite site, string pageTitle, int? defaultNamespaceId = null)
        {
            return await BackLinks(site, pageTitle, MaxCount, defaultNamespaceId, CancellationToken.None);
        }

        public static async Task<IList<SearchResultEntry>> BackLinks(this WikiSite site, string pageTitle, int maxCount,
    int? defaultNamespaceId, CancellationToken cancellationToken)
        {
            int offset = 0;
            string continueToken = null;
            var results = new JArray();
            while (offset >= 0)
            {
                var payload = new ExpandoObject();

                payload.TryAdd("action", "query");
                payload.TryAdd("list", "backlinks");
                payload.TryAdd("bltitle", pageTitle);
                payload.TryAdd("bllimit", maxCount);

                if(defaultNamespaceId.HasValue)
                {
                    payload.TryAdd("blnamespace", defaultNamespaceId.Value);
                }
                if(continueToken != null)
                {
                    payload.TryAdd("blcontinue", continueToken);
                }
                var jresult = await site.InvokeMediaWikiApiAsync(new MediaWikiFormRequestMessage(payload.ToList()), cancellationToken);

                var query = jresult["query"];
                results.Merge((JArray)query["backlinks"]);
                var querycontinue = jresult["query-continue"];
                if (querycontinue != null)
                {
                    continueToken = querycontinue["backlinks"]["blcontinue"].ToString();
                }
                else
                {
                    offset = -1;
                }
            }
            var result = new List<SearchResultEntry>();
            foreach (JToken token in results)
            {
                var entry = new SearchResultEntry();
                entry.Title = token["title"].ToString();
                entry.NamespaceID = int.Parse(token["ns"].ToString());
                result.Add(entry);
            }

            return result;
        }

        /// <summary>
        /// Options for search.
        /// </summary>
        [Flags]
        public enum SearchOptions
        {
            /// <summary>No options.</summary>
            title = 0,

            /// <summary>
            /// Return the target page when meeting redirects.
            /// This may cause OpenSearch return fewer results than limitation.
            /// </summary>
            text = 1,
        }

        /// <summary>
        /// Represents an entry in opensearch result.
        /// </summary>
        public struct SearchResultEntry
        {
            /// <summary>
            /// Title of the page.
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// Url of the page. May be null.
            /// </summary>
            public int NamespaceID { get; set; }

            ///<summary>
            ///Translates the namespace id into the corresponding enum.
            ///</summary>
            ///<returns>
            /// BuiltInNameSpaces
            ///</returns>
            public string GetNamespaceString()
            {
                return BuiltInNamespaces.GetCanonicalName(NamespaceID);
            }
            /// <summary>
            /// Returns a string representation of the object
            /// </summary>
            /// <returns>
            /// Title + NamespaceID <see cref="T:System.String"/>。
            /// </returns>
            public override string ToString()
            {
                return Title + NamespaceID;
            }
        }

    }
}
