//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using WikiClientLibrary.Generators;
//using WikiClientLibrary.Sites;

//namespace Wikibot.Logic.Extensions
//{
//    public class TFWikiRecentChangesGenerator : RecentChangesGenerator
//    {
//        public TFWikiRecentChangesGenerator(WikiSite site) : base(site)
//        {

//        }

//        private IEnumerable<KeyValuePair<string, object?>> EnumParams(bool isList)
//    => base.EnumParams(isList).Select(p => p.Key == "rvprop" ? new KeyValuePair<string, object?>(p.Key, ((string)p.Value).Replace("|tags", "")) : p);
//    }
//}
