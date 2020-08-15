using LinqToWiki.Generated;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Wikibot.App.Models.Jobs;
using WikiClientLibrary;
using WikiClientLibrary.Client;
using WikiClientLibrary.Sites;

namespace Wikibot.App.Jobs
{
    public class AbstractJob : WikiJob
    {
        public IConfiguration Configuration;

        [NotMapped]
        public Serilog.ILogger Log { get; set; }

        private DbContextOptions _dboptions;
        public DbContextOptions DBOptions
        {
            get
            {
                if(_dboptions == null)
                {
                    var builder = new SqlConnectionStringBuilder(Configuration.GetConnectionString("JobDB"));
                    builder.Password = Configuration.GetSection("JobDb")["DbPassword"];
                    _dboptions = new DbContextOptionsBuilder().UseSqlServer(builder.ConnectionString).Options;
                }
                return _dboptions;
            }
        }

        public IConfigurationSection WikiConfig
        {
            get
            {
                return Configuration.GetSection("WikiLogin");
            }
        }
        private Wiki _wiki;
        public Wiki Wiki
        {
            get
            {
                if (_wiki == null)
                {
                    var wikiLoginConfig = Configuration.GetSection("WikiLogin");
                    var username = WikiConfig["Username"];
                    var password = WikiConfig["Password"];
                    _wiki = new Wiki("WikiBot", "https://tfwiki.net", "/mediawiki/api.php");
                    var result = _wiki.login(username, password);

                    if (result.result == loginresult.NeedToken)
                        result = _wiki.login(username, password, token: result.token);

                    if (result.result == loginresult.Success)
                        return _wiki;
                    else
                    {
                        Log.Error("Login for Wiki failed with result {result}", result.result.ToString());
                        throw new Exception(result.result.ToString());
                    }
                }
                else
                    return _wiki;
            }
        }

        private WikiClient _client;
        public WikiClient Client
        {
            get
            {
                if (_client == null)
                {
                    var client = new WikiClient
                    {
                        ClientUserAgent = "WCLQuickStart/1.0 (your user name or contact information here)"
                    };
                    _client = client;
                }
                return _client;
            }
        }

        private WikiSite _site;
        public WikiSite Site
        {
            get {
                if (_site == null)
                {
                    var username = WikiConfig["Username"];
                    var password = WikiConfig["Password"];
                    var url = WikiConfig["APIUrl"];
                    // You can create multiple WikiSite instances on the same WikiClient to share the state.
                    _site = new WikiSite(Client, url);

                    try
                    {
                        // Wait for initialization to complete.
                        // Throws error if any.
                        _site.Initialization.Wait();
                        
                        _site.LoginAsync(username, password).Wait();
                    }
                    catch (WikiClientException ex)
                    {
                        Log.Error(ex, "Error occurred while initializing or logging into WikiSite");
                        // Add your exception handler for failed login attempt.
                        throw;
                    }
                }
                return _site;
            }
        }

        public void SetJobStart()
        {
            if (Status == JobStatus.PreApproved)
            {
                TimePreStarted = DateTime.UtcNow;
                Log.Information("Job started at {DateTime}", TimePreStarted);
            }
            else
            {
                TimeStarted = DateTime.UtcNow;
                Log.Information("Job started at {DateTime}", TimeStarted);
            }
        }

        public void SetJobEnd()
        {
            if (Status == JobStatus.PreApproved)
            {
                TimePreFinished = DateTime.UtcNow;
                Status = JobStatus.PendingApproval;
                Log.Information("Job ended at {DateTime}", TimePreFinished);
            }
            else if(Status == JobStatus.Approved)
            {
                TimeFinished = DateTime.UtcNow;
                Status = JobStatus.Done;
                Log.Information("Job ended at {DateTime}", TimeFinished);
            }

        }

        public void SaveJob()
        {
            Log.Information("Saving job.");
            using (JobContext context = new JobContext(DBOptions))
            {
                context.Jobs.Update(this);
                context.SaveChanges();
            }
        }

        public void CleanUp()
        {
            Log.Information("Cleaning up.");
            _client.Dispose();
        }
    }
}
