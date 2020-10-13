using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Wikibot.App.Models
{
    public interface IJwtFactory
    {
        ClaimsIdentity GenerateClaimsIdentity(string userName, string id, IList<string> roles);
        Task<string> GenerateEncodedToken(string userName, ClaimsIdentity identity);
    }
}