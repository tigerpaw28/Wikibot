using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations.Schema;
using Wikibot.DataAccess;

namespace Wikibot.Logic.Jobs
{
    public class AbstractJob
    {
        
        [NotMapped]
        public IConfiguration Configuration { get; set; }

        [NotMapped]
        public Serilog.ILogger Log { get; set; }

        [NotMapped]
        public RequestData JobData { get; set; }

        [NotMapped]
        public bool UsePendingPreApproval 
        {
            get {
                return bool.Parse(Configuration["EnablePendingPreApproval"]);
            }
        }

        public virtual void Execute() { }

    }
}
