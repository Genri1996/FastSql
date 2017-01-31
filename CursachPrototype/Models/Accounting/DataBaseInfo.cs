﻿using System;
using DataProxy;

namespace CursachPrototype.Models.Accounting
{
    public class DataBaseInfo
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime DateOfCreating { get; set; }

        public string ConnectionString { get; set; }

        public DbmsType DbmsType { get; set; }

        public string ForeignKey { get; set; }
    }
}