using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace CursachPrototype.Models.Accounting
{
    /// <summary>
    /// From metanit.com. 
    /// Custom app user manager
    /// </summary>
    public class AppUserManager : UserManager<AppUser>
    {
        public AppUserManager(IUserStore<AppUser> store) : base(store)
        {
        }

        public static AppUserManager Create(IdentityFactoryOptions<AppUserManager> options, IOwinContext context)
        {
            AppIdentityContext db = context.Get<AppIdentityContext>();
            AppUserManager manager = new AppUserManager(new UserStore<AppUser>(db));
            return manager;
        }
    }
}