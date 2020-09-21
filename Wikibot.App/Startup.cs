using FluentScheduler;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Wikibot.App.Data;
using Wikibot.Logic.JobRetrievers;
using Wikibot.Logic.Logic;
using Wikibot.App.Services;
using Wikibot.DataAccess;

namespace Wikibot.App
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {

            Configuration = configuration;
            JobManager.Initialize(new Registry());
            JobManager.UseUtcTime();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var builder = new SqlConnectionStringBuilder(
            Configuration.GetConnectionString("JobDB"));
            builder.Password = Configuration.GetSection("JobDb")["DbPassword"];

            //TODO: Add seperate connection string for this context
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                            builder.ConnectionString
                )
            ) ;
            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddControllersWithViews();
            services.AddRazorPages();

            var logConfig = new LoggerConfiguration().ReadFrom.Configuration(Configuration);
            var logger = logConfig.CreateLogger();
            services.AddSingleton<Serilog.ILogger>(logger);

            //services.AddTransient<IEmailSender, EmailSender>();
            services.AddTransient<IWikiAccessLogic, WikiAccessLogic>();
            services.AddTransient<IWikiJobRetriever, TFWikiJobRetriever>();
            services.AddTransient<IDataAccess, SqlDataAccess>();

            services.Configure<AuthMessageSenderOptions>(Configuration);

           

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseSerilogRequestLogging();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
