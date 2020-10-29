using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(Wikibot.App.Areas.Identity.IdentityHostingStartup))]
namespace Wikibot.App.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}