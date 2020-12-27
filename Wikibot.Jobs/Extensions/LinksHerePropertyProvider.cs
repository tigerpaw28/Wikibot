using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using WikiClientLibrary;
using WikiClientLibrary.Generators;
using WikiClientLibrary.Infrastructures;
using WikiClientLibrary.Pages;
using WikiClientLibrary.Pages.Queries.Properties;


namespace Wikibot.Logic.Extensions
{
    public class LinksHerePropertyProvider : WikiPagePropertyProvider<LinksHerePropertyGroup>
    {

        /// <summary>
        /// Whether to include hidden categories in the returned list.
        /// </summary>
        public PropertyFilterOption RedirectFilter { get; set; }

        /// <summary>
        /// Only list these categories. Useful for checking whether a certain page is in a certain category.
        /// </summary>
        public IEnumerable<int> NamespaceSelection { get; set; }

        /// <inheritdoc />
        public override IEnumerable<KeyValuePair<string, object>> EnumParameters(MediaWikiVersion version)
        {
            var p = new OrderedKeyValuePairs<string, object>
            {
                {"lhprop", "pageid|title|redirect"},
                {"lhshow", RedirectFilter.ToString("redirect", "!redirect", null)}
            };
            if (NamespaceSelection != null) p.Add("lhnamespace", MediaWikiHelper.JoinValues(NamespaceSelection));
            return p;
        }

        /// <inheritdoc />
        public override LinksHerePropertyGroup ParsePropertyGroup(JObject json)
        {
            if (json == null) throw new ArgumentNullException(nameof(json));
            return LinksHerePropertyGroup.Create((JArray)json["links"]);
        }

        /// <inheritdoc />
        public override string PropertyName => "links";
    }

    /// <summary>
    /// Contains information about a page's belonging category.
    /// </summary>
    public struct WikiPageLinksHereInfo
    {
        public WikiPageLinksHereInfo(WikiPageStub page, bool isRedirect)
        {
            Page = page;
            IsRedirect = isRedirect;
            //SortKey = sortKey;
            //TimeStamp = timeStamp;
            //FullSortKey = fullSortKey;
        }

        /// <summary>
        /// Page information for the category.
        /// </summary>
        /// <remarks>The <see cref="WikiPageStub.HasId"/> of the returned <see cref="WikiPageStub"/> is <c>false</c>.</remarks>
        public WikiPageStub Page { get; }

        /// <summary>
        /// Full name of the category.
        /// </summary>
        public string Title => Page.HasTitle ? Page.Title : null;

        /// <summary>Gets a value that indicates whether the category is hidden.</summary>
        public bool IsRedirect { get; }

        ///// <summary>Gets the sortkey prefix (human-readable part) of the current page in the category.</summary>
        //public string SortKey { get; }

        ///// <summary>Gets the sortkey (hexadecimal string) of the current page in the category.</summary>
        //public string FullSortKey { get; }

        ///// <summary>Gets the timestamp of when the category was added.</summary>
        ///// <value>A timestamp, or <see cref="DateTime.MinValue"/> if not available.</value>
        //public DateTime TimeStamp { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            return Page.ToString();
        }
    }

    public class LinksHerePropertyGroup : WikiPagePropertyGroup
    {

        private static readonly LinksHerePropertyGroup empty = new LinksHerePropertyGroup();

        internal static LinksHerePropertyGroup Create(JArray jcats)
        {
            if (jcats == null) return null;
            if (!jcats.HasValues) return empty;
            return new LinksHerePropertyGroup(jcats);
        }

        private LinksHerePropertyGroup()
        {
            LinkedPages = new WikiPageLinksHereInfo[0];
        }

        private LinksHerePropertyGroup(JArray jcats)
        {
            LinkedPages = new ReadOnlyCollection<WikiPageLinksHereInfo>(jcats.Select(LinkedPagesFromJson).ToList());
        }

        internal static WikiPageLinksHereInfo LinkedPagesFromJson(JToken json)
        {
            return new WikiPageLinksHereInfo(MediaWikiHelper.PageStubFromJson((JObject)json),
                json["redirect"] != null);
        }

        public IReadOnlyCollection<WikiPageLinksHereInfo> LinkedPages { get; }

    }
}

