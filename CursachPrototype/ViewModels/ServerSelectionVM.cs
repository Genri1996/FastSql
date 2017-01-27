using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace CursachPrototype.ViewModels
{
    public class ServerSelectionVm
    {
        [HiddenInput(DisplayValue = false)]
        public List<String>  AvailableServers { get; set; }

        [Required(ErrorMessage = "Поле должно быть выбрано.")]
        [Display(Name = "Тип СУБД")]
        public String SelectedServer { get; set; }

        [Required(ErrorMessage = "Название БД должно быть установленно.")]
        [Display(Name = "Название")]
        [RegularExpression(@"[A-z]{3,40}", ErrorMessage = "Некорректное имя БД")]
        public String DataBaseName { get; set; }

        [Display(Name = "Пароль к базе данных")]
        public String DataBasePassword { get; set; }
    }
}