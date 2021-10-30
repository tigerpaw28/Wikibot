using System.Collections.Generic;
using System.Threading.Tasks;
using Wikibot.DataAccess;
using Wikibot.Logic.Extensions;

namespace Wikibot.Logic
{
    public interface INotificationService
    {
        Task<List<EmailResult>> SendErrorNotification(List<User> reviewers, string requestingUsername, string comment);
        Task<List<EmailResult>> SendNewApprovalNotification(List<User> reviewers, string requestingUsername, string comment);
        Task<List<EmailResult>> SendNewRequestNotification(List<User> reviewers, string requestingUsername, string comment);
        Task<EmailResult> SendRequestApprovedNotification(string requestingUsername, string comment, string requestType);
        Task<EmailResult> SendRequestCompletedNotification(string requestingUsername, string comment, string requestType);
        Task<EmailResult> SendRequestPreApprovedNotification(string requestingUsername, string comment, string requestType);
        Task<EmailResult> SendRequestRejectedNotification(string requestingUsername, string comment, string requestType, string reviewerComment, string reviewerName);
    }
}