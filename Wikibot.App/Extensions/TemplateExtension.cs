using MwParserFromScratch.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Wikibot.App.Jobs;

namespace Wikibot.App.Extensions
{
    public static class TemplateExtension
    {
        public static bool EqualsJob(this Template template, WikiJob job)
        {
            return (template.ToString().Equals(job.RawRequest));
        }
    }
}
