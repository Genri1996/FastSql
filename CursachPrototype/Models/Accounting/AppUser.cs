using System.Collections.Generic;
using Microsoft.AspNet.Identity.EntityFramework;

namespace CursachPrototype.Models.Accounting
{
    public class AppUser : IdentityUser
    {
        public string UserNickName { get; set; }
    }
}