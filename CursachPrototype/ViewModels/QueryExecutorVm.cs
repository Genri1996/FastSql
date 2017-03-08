using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using DataProxy;

namespace CursachPrototype.ViewModels
{
    public class QueryExecutorVm
    {
        public int DbId { get; set; }
        public string DbName { get; set; }
        public string Query { get; set; }
        public DbmsType DbmsType { get; set; }
        public List<DbmsType> AvailableServers { get; } = DataService.AvailableServers;
        public DataTable DataTable { get; set; }

    }
}