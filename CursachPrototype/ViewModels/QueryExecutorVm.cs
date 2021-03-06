﻿using System.Data;

namespace CursachPrototype.ViewModels
{
    public class QueryExecutorVm
    {
        public int DbId { get; set; }
        public string DbName { get; set; }
        public string Query { get; set; }
        public DataTable DataTable { get; set; } = null;
    }
}