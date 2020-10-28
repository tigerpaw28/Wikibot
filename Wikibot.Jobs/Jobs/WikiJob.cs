using Microsoft.Extensions.Configuration;
using MwParserFromScratch;
using MwParserFromScratch.Nodes;
using System;
using System.Linq;
using Wikibot.DataAccess;
using Wikibot.DataAccess.Objects;

namespace Wikibot.Logic.Jobs
{
    public abstract class WikiJob : AbstractJob
    {
        public WikiJobRequest Request { get; set; }

        public IConfigurationSection WikiConfig
        {
            get
            {
                return Configuration.GetSection("WikiLogin");
            }
        }

        private string _fromText;
        public string FromText 
        { 
            get
            {
                if (_fromText == null)
                {
                    var template = new WikitextParser().Parse(Request.RawRequest).Lines.SelectMany(x => x.EnumDescendants().OfType<Template>()).Single();
                    _fromText =  template.Arguments.Single(arg => arg.Name.ToPlainText() == "before").Value.ToPlainText();
                }
                return _fromText;
            }
        }

        private string _toText;
        public string ToText
        {
            get
            {
                if (_toText == null)
                {
                    var template = new WikitextParser().Parse(Request.RawRequest).Lines.SelectMany(x => x.EnumDescendants().OfType<Template>()).Single();
                    _toText = template.Arguments.Single(arg => arg.Name.ToPlainText() == "after").Value.ToPlainText();
                }
                return _toText;
            } 
            
        }

        public void SetJobStart()
        {
            if (Request.Status == JobStatus.PreApproved)
            {
                Request.TimePreStartedUTC = DateTime.UtcNow;
                Log.Information("Job started at {DateTime}", Request.TimePreStartedUTC);
            }
            else
            {
                Request.TimeStartedUTC = DateTime.UtcNow;
                Log.Information("Job started at {DateTime}", Request.TimeStartedUTC);
            }
        }

        public void SetJobEnd()
        {
            if (Request.Status == JobStatus.PreApproved)
            {
                Request.TimePreFinishedUTC = DateTime.UtcNow;
                Request.Status = JobStatus.PendingApproval;
                Log.Information("Job ended at {DateTime}", Request.TimePreFinishedUTC);
            }
            else if (Request.Status == JobStatus.Approved)
            {
                Request.TimeFinishedUTC = DateTime.UtcNow;
                Request.Status = JobStatus.Done;
                Log.Information("Job ended at {DateTime}", Request.TimeFinishedUTC);
            }

        }

        public void SaveRequest()
        {
            Log.Information("Saving job.");

            JobData.UpdateStatus(this.Request.ID, this.Request.Status);
        }
    }
}
