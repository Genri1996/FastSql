using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CursachPrototype.ViewModels
{
    public class DbVm
    {
        [HiddenInput(DisplayValue = false)]
        public List<string> AvailableServers { get; set; }

        [Required(ErrorMessage = "Поле должно быть выбрано.")]
        [Display(Name = "Тип СУБД")]
        public string SelectedServer { get; set; }

        [Required(ErrorMessage = "Название БД должно быть установленно.")]
        [Display(Name = "Название")]
        [RegularExpression(@"[a-zA-Z0-9]{3,40}", ErrorMessage = "Некорректное имя БД")]
        public string DataBaseName { get; set; }

        public virtual bool IsPublic => true;
    }
}