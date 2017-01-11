using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace CursachPrototype.ViewModels
{
    public class ServerSelectionVM
    {
        [HiddenInput(DisplayValue = false)]
        public List<String>  AvailableServers { get; set; }

        [Required(ErrorMessage = "Поле должно быть установленно.")]
        [Display(Name = "Тип СУБД")]
        public String SelectedServer { get; set; }

        [Required(ErrorMessage = "Имя БД должно быть установленно")]
        [Display(Name = "Название")]
        public String DataBaseName { get; set; }
    }
}