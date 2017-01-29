﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProxy.Helpers
{
    public interface IHelper
    {
        bool IsDataBaseExists(String dbName);

        bool DropDataBase(String dbName);

        bool IsLoginExists(String login);
    }
}
