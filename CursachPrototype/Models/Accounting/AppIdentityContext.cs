﻿using DataProxy.DbManangment;
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
            
        }

        public static AppIdentityContext Create()
        {
            return new AppIdentityContext();
        }
    }
}