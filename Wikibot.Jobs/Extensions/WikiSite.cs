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

        /// <summary>
        /// Get a list of backlinks for a given page. Backlinks are pages that link to a page. 
        /// </summary>
        /// <param name="pageTitle">Name of the page to get backlinks for.</param>
        /// <param name="defaultNamespaceId">The namespace in which to look for the page.</param>
        /// <returns>List of pages.</returns>
        public static async Task<IList<SearchResultEntry>> BackLinks(this WikiSite site, string pageTitle, int? defaultNamespaceId = null)
        {
            return await BackLinks(site, pageTitle, MaxCount, defaultNamespaceId, CancellationToken.None);
        }

        /// <summary>
        /// Get a list of backlinks for a given page. Backlinks are pages that link to a page. 
        /// </summary>
        /// <param name="pageTitle">Name of the page to get backlinks for.</param>
        /// <param name="maxCount">Maximum number of results to return. No more than 500 (5000 for bots) allowed.</param>
        /// <param name="defaultNamespaceId">The namespace in which to look for the page.</param>
        /// <param name="cancellationToken">The cancellation token that will be checked prior to completing the returned task.</param>
        /// <returns>List of pages.</returns>
        public static async Task<IList<SearchResultEntry>> BackLinks (this WikiSite site, string pageTitle, int maxCount,
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
        /// Sends an email to a user via API:Emailuser
        /// </summary>
        /// <param name="target">The username of the user to send the email to.</param>
        /// <param name="text">The body of the email. This is treated as plaintext by the API.</param>
        /// <param name="cancellationToken">The cancellation token that will be checked prior to completing the returned task.</param>
        /// <param name="subject">The subject of the email. (optional)</param>
        /// <param name="ccme">Should the sending user be CC'd? (optional)</param>
        /// <returns>EmailResult</returns>
        public static async Task<EmailResult> SendEmailToUser(this WikiSite site, string target, string text, CancellationToken cancellationToken, string subject = null, bool? ccme = null)
        {
            string emailToken = site.GetTokenAsync("email").Result;

            var payload = new ExpandoObject();

            payload.TryAdd("action", "emailuser");
            payload.TryAdd("target", target);           
            payload.TryAdd("text", text);
            payload.TryAdd("token", emailToken);
            payload.TryAdd("format", "json");

            if (!string.IsNullOrEmpty(subject))
            {
                payload.TryAdd("subject", subject ?? "");
            }

            if(ccme.HasValue && ccme.Value)
            {
                payload.TryAdd("ccme", ccme);
            }

            var jresult = await site.InvokeMediaWikiApiAsync(new MediaWikiFormRequestMessage(payload.ToList()), cancellationToken);
            string result = jresult["emailuser"]["result"].ToString();

            var emailResult = new EmailResult(result, (result.Equals("Success")));

            return emailResult;
        }

        /// <summary>
        /// Sends an email to a user via API:Emailuser
        /// </summary>
        /// <param name="target">The username of the user to send the email to.</param>
        /// <param name="text">The body of the email. This is treated as plaintext by the API.</param>
        /// <param name="subject">The subject of the email. (optional)</param>
        /// <param name="ccme">Should the sending user be CC'd? (optional)</param>
        /// <returns>EmailResult</returns>
        public static async Task<EmailResult> SendEmailToUser(this WikiSite site, string target, string text, string subject = null, bool? ccme = null)
        {
            return await SendEmailToUser(site, target, text, CancellationToken.None, subject, ccme);
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
