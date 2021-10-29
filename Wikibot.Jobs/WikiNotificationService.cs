using LinqToWiki.Generated;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Wikibot.DataAccess;
using Wikibot.Logic.Extensions;
using Wikibot.Logic.Logic;
using WikiClientLibrary.Client;

namespace Wikibot.Logic
{
    public class WikiNotificationService : INotificationService
    {
        private IWikiAccessLogic _accessLogic;
        private Dictionary<string, string> _subjects;
        public WikiNotificationService(IWikiAccessLogic wikiAccessLogic, IConfiguration config)
        {
            _accessLogic = wikiAccessLogic;
            _subjects = new Dictionary<string, string>();
            _subjects["NewRequest"] = config.GetSection("EmailSubjects").GetValue<string>("NewRequest");
            _subjects["NewApproval"] = config.GetSection("EmailSubjects").GetValue<string>("NewApproval");
            _subjects["Error"] = config.GetSection("EmailSubjects").GetValue<string>("Error");
            _subjects["RequestComplete"] = config.GetSection("EmailSubjects").GetValue<string>("RequestComplete");
            _subjects["RequestPreApproved"] = config.GetSection("EmailSubjects").GetValue<string>("RequestPreApproved");
            _subjects["RequestApproved"] = config.GetSection("EmailSubjects").GetValue<string>("RequestApproved");
            _subjects["RequestRejected"] = config.GetSection("EmailSubjects").GetValue<string>("RequestRejected");

        }
        public async Task<List<EmailResult>> SendNewRequestNotification(List<User> reviewers, string requestingUsername, string comment)
        {
            //Determine how to provide a persistent token in email link. Probably just need to generate token with longer expiration time.
            var results = new List<EmailResult>();
            using (var client = new WikiClient()
            {
                ClientUserAgent = "WCLQuickStart/1.0 (your user name or contact information here)"
            })
            {
                var site = _accessLogic.GetLoggedInWikiSite(client);
                foreach (User reviewer in reviewers)
                {
                    StringBuilder template = new();
                    template.Append("Hi, ").Append(reviewer.Username).AppendLine(",");
                    template.AppendLine();
                    template.Append("A new request is awaiting your pre-approval:");
                    template.AppendLine();
                    template.Append("    Requesting User: ").AppendLine(requestingUsername);
                    template.Append("    Comment: ").AppendLine(comment);
                
                    var emailResult = await site.SendEmailToUser(reviewer.Username, template.ToString(), _subjects["NewRequest"]); 
                    results.Add(emailResult);
                }
            }

            return results;
        }

        public async Task<List<EmailResult>> SendNewApprovalNotification(List<User> reviewers, string requestingUsername, string comment)
        {
            var results = new List<EmailResult>();
            using (var client = new WikiClient()
            {
                ClientUserAgent = "WCLQuickStart/1.0 (your user name or contact information here)"
            })
            {
                var site = _accessLogic.GetLoggedInWikiSite(client);
                foreach (User reviewer in reviewers)
                {
                    StringBuilder template = new();
                    template.Append("Hi, ").Append(reviewer.Username).Append(",");
                    template.AppendLine();
                    template.Append("A request is awaiting your approval:");
                    template.AppendLine();
                    template.Append("    Requesting User: ").AppendLine(requestingUsername);
                    template.Append("    Comment: ").AppendLine(comment);

                    var emailResult = await site.SendEmailToUser(reviewer.Username, template.ToString(), _subjects["NewApproval"]);
                    results.Add(emailResult);
                }
            }

            return results;
        }

        public async Task<List<EmailResult>> SendErrorNotification(List<User> reviewers, string requestingUsername, string comment)
        {
            var results = new List<EmailResult>();
            using (var client = new WikiClient()
            {
                ClientUserAgent = "WCLQuickStart/1.0 (your user name or contact information here)"
            })
            {
                var site = _accessLogic.GetLoggedInWikiSite(client);
                foreach (User reviewer in reviewers)
                {
                    StringBuilder template = new();
                    template.Append("Hi, ").Append(reviewer.Username).AppendLine(",");
                    template.AppendLine();
                    template.Append("An error occurred while processing the below request:");
                    template.AppendLine();
                    template.Append("    Requesting User: ").AppendLine(requestingUsername);
                    template.Append("    Comment: ").AppendLine(comment);

                    var emailResult = await site.SendEmailToUser(reviewer.Username, template.ToString(), _subjects["Error"]);
                    results.Add(emailResult);
                }
            }

            return results;
        }

        public async Task<EmailResult> SendRequestCompletedNotification(string requestingUsername, string comment, string requestType )
        {
            EmailResult result = null;
            using (var client = new WikiClient()
            {
                ClientUserAgent = "WCLQuickStart/1.0 (your user name or contact information here)"
            })
            {
                var site = _accessLogic.GetLoggedInWikiSite(client);
                StringBuilder template = new();
                template.Append("Hi, ").Append(requestingUsername).AppendLine(",");
                template.AppendLine();
                template.Append("Your request has been completed:");
                template.AppendLine();
                template.Append("    Request type: ").AppendLine(requestType);
                template.Append("    Your Comment: ").AppendLine(comment);

                result = await site.SendEmailToUser(requestingUsername, template.ToString(), _subjects["RequestComplete"]);
            }

            return result;
        }

        public async Task<EmailResult> SendRequestPreApprovedNotification(string requestingUsername, string comment, string requestType)
        {
            EmailResult result = null;
            using (var client = new WikiClient()
            {
                ClientUserAgent = "WCLQuickStart/1.0 (your user name or contact information here)"
            })
            {
                var site = _accessLogic.GetLoggedInWikiSite(client);
                StringBuilder template = new();
                template.Append("Hi, ").Append(requestingUsername).Append(",");
                template.AppendLine();
                template.Append("Your request has been approved for mock-up:");
                template.AppendLine();
                template.Append("    Request type: ").AppendLine(requestType);
                template.Append("    Your Comment: ").AppendLine(comment);

                 result = await site.SendEmailToUser(requestingUsername, template.ToString(), _subjects["RequestPreApproved"]);
            }

            return result;
        }

        public async Task<EmailResult> SendRequestApprovedNotification(string requestingUsername, string comment, string requestType)
        {
            EmailResult result = null;
            using (var client = new WikiClient()
            {
                ClientUserAgent = "WCLQuickStart/1.0 (your user name or contact information here)"
            })
            {
                var site = _accessLogic.GetLoggedInWikiSite(client);
                StringBuilder template = new();
                template.Append("Hi, ").Append(requestingUsername).AppendLine(",");
                template.AppendLine();
                template.Append("The proposed changes for your request have been approved:");
                template.AppendLine();
                template.Append("    Request type: ").AppendLine(requestType);
                template.Append("    Your Comment: ").AppendLine(comment);

                result = await site.SendEmailToUser(requestingUsername, template.ToString(), _subjects["RequestApproved"]);
            }

            return result;
        }

        public async Task<EmailResult> SendRequestRejectedNotification(string requestingUsername, string comment, string requestType, string reviewerComment, string reviewerName)
        {
            EmailResult result = null;
            using (var client = new WikiClient()
            {
                ClientUserAgent = "WCLQuickStart/1.0 (your user name or contact information here)"
            })
            {
                var site = _accessLogic.GetLoggedInWikiSite(client);
                StringBuilder template = new();
                template.Append("Hi, ").Append(requestingUsername).AppendLine(",");
                template.AppendLine();
                template.AppendLine("Your request has been rejected:");
                template.AppendLine();
                template.Append("    Request type: ").AppendLine(requestType);
                template.Append("    Your Comment: ").AppendLine(comment);
                template.Append("    Reviewer: ").AppendLine(reviewerName);
                template.Append("    Reviewer Comments: ").AppendLine(reviewerComment);

                result = await site.SendEmailToUser(requestingUsername, template.ToString(), _subjects["RequestRejected"]);
            }

            return result;
        }
    }
}
