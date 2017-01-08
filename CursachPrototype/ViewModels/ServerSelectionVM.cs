using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CursachPrototype.ViewModels
{
    public class ServerSelectionVM
    {
        public List<String>  AvailableServers { get; set; }
        [Required]
        public String SelectedServer { get; set; }
        [Required]
        public String DataBaseName { get; set; }
    }
}