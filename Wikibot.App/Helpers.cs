using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wikibot.App
{
    public class Helpers
    {
        public static class Constants
        {
            public static class Strings
            {
                public static class JwtClaimIdentifiers
                {
                    public const string Role = "role", Id = "id";
                }

                public static class JwtClaims
                {
                    public const string ApiAccess = "BotAdmin";
                }
            }
        }

        public static class Errors
        {
            public static ModelStateDictionary AddErrorsToModelState(IdentityResult identityResult, ModelStateDictionary modelState)
            {
                foreach (var e in identityResult.Errors)
                {
                    modelState.TryAddModelError(e.Code, e.Description);
                }

                return modelState;
            }

            public static ModelStateDictionary AddErrorToModelState(string code, string description, ModelStateDictionary modelState)
            {
                modelState.TryAddModelError(code, description);
                return modelState;
            }
        }
    }
}
