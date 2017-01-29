using CursachPrototype.ExtensionMethods;
using DataProxy.Helpers;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity.EntityFramework;

namespace CursachPrototype.Models.Accounting
{
    public class AppIdentityContext:IdentityDbContext<AppUser>
    {
        public AppIdentityContext() : base("IdentityDb")
        {
            Database.CreateIfNotExists();
        }

        public static AppIdentityContext Create()
        { 
            //Create users table
        


            return new AppIdentityContext();
        }
    }
}