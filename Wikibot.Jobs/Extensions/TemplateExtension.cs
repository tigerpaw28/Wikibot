using MwParserFromScratch.Nodes;
using Wikibot.DataAccess.Objects;

namespace Wikibot.Logic.Extensions
{
    public static class TemplateExtension
    {
        public static bool EqualsJob(this Template template, WikiJobRequest job)
        {
            return (template.ToString().Equals(job.RawRequest));
        }
    }
}
