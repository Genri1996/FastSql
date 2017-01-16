using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace CursachPrototype.Models
{
    public class AppIdentityContext:IdentityDbContext<AppUser>
    {
        public AppIdentityContext() : base("IdentityDb") { }

        public static AppIdentityContext Create()
        {
            return new AppIdentityContext();
        }
    }
}