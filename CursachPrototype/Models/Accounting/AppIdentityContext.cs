﻿using Microsoft.AspNet.Identity.EntityFramework;

namespace CursachPrototype.Models.Accounting
{
    public class AppIdentityContext:IdentityDbContext<AppUser>
    {
        public AppIdentityContext() : base("IdentityDb")
        {
            Database.CreateIfNotExists();
            //Create table DbInfos
            DataBaseInfoManager.CreateTableIfNotExists();
        }

        public static AppIdentityContext Create()
        { 
            return new AppIdentityContext();
        }
    }
}