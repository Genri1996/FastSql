using System.Configuration;
using Microsoft.Owin;
using Owin;
using CursachPrototype.Models.Accounting;
using DataProxy;
using DataProxy.DbManangment;
using Hangfire;
using Microsoft.Owin.Security.Cookies;
using Microsoft.AspNet.Identity;

[assembly: OwinStartup(typeof(CursachPrototype.Startup))]

namespace CursachPrototype
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            DataService.ExecuteQueryAsString("USE MASTER CREATE DATABASE HANGFIRE",
                ConfigurationManager.ConnectionStrings["SqlServerMaster"].ConnectionString, DbmsType.SqlServer);

            GlobalConfiguration.Configuration.UseSqlServerStorage("Hangfire");

            app.UseHangfireDashboard();
            app.UseHangfireServer();

            RecurringJob.AddOrUpdate(() => DataBasesManager.DropOutdatedDbs(), Cron.Hourly);

            // настраиваем контекст и менеджер
            app.CreatePerOwinContext<AppIdentityContext>(AppIdentityContext.Create);
            app.CreatePerOwinContext<AppUserManager>(AppUserManager.Create);
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login")
            });
        }
    }
}