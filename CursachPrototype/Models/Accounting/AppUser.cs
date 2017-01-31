using Microsoft.AspNet.Identity.EntityFramework;

namespace CursachPrototype.Models.Accounting
{
    /// <summary>
    /// Custom app user
    /// </summary>
    public class AppUser : IdentityUser
    {
        /// <summary>
        /// Contains a unick user nickname. For unic DBs names.
        /// </summary>
        public string UserNickName { get; set; }
    }
}