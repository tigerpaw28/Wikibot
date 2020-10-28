using MwParserFromScratch.Nodes;
using System.Linq;
using Wikibot.DataAccess.Objects;

namespace Wikibot.Logic.Extensions
{
    public static class TemplateExtension
    {
        public static bool EqualsJob(this Template template, WikiJobRequest job)
        {
            
            var templateID = template.Arguments.SingleOrDefault(arg => arg.Name.ToPlainText().Equals("id"));
            bool hasMatchingID = (templateID == null || templateID.Value.ToPlainText().Equals(job.ID.ToString()));
            return (template.ToString().Equals(job.RawRequest) && hasMatchingID);
        }
    }
}
