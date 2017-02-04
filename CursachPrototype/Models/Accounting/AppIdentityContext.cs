using DataProxy.DbManangment;
using Hangfire;
using Microsoft.AspNet.Identity.EntityFramework;

namespace CursachPrototype.Models.Accounting
{
    public class AppIdentityContext : IdentityDbContext<AppUser>
    {
        public AppIdentityContext() : base("IdentityDb")
        {
            Database.CreateIfNotExists();
            //Create table DbInfos
            DataBasesManager.CreateTablesIfNotExists();
            RecurringJob.AddOrUpdate(() => DataBasesManager.DropOutdatedDbs(), Cron.Minutely);//TODO: Hourly
        }

        public static AppIdentityContext Create()
        {
            return new AppIdentityContext();
        }
    }
}