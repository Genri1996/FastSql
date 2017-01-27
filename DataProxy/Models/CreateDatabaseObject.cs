using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProxy.Models
{
    public class CreateDatabaseObject
    {
        public DbmsType SelectedDbms { get; set; }
    
        public String DataBaseName { get; set; }

        public bool IsProtectionRequired { get; set; } = false;

        public String DataBaseLogin { get; set; }

        public String DataBasePassword { get; set; }

       
    }
}
