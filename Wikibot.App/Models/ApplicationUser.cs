
using Microsoft.AspNetCore.Identity;
using System;
using System.Security.Claims;


namespace Wikibot.App.Models
{
    public class ApplicationUser: IdentityUser //ApplicationUser<string>
    { }
    //public class ApplicationUser<TKey> : IdentityUser<TKey> where TKey : IEquatable<TKey>
    //{
    //    //public long Id { get; set; }
    //}
}
