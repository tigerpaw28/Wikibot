using LinqToWiki.Generated;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Wikibot.App.Data;
using WikiClientLibrary;
using WikiClientLibrary.Client;
using WikiClientLibrary.Sites;

namespace Wikibot.App.Jobs
{
    public abstract class WikiJob : AbstractJob
    {
        public JobType RequestType { get; set; }
        public string RawRequest { get; set; }
        public string ProposedChanges { get; set; }
        public string Notes { get; set; }

        public IConfigurationSection WikiConfig
        {
            get
            {
                return Configuration.GetSection("WikiLogin");
            }
        }
        
    }
}
