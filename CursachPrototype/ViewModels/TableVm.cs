using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CursachPrototype.ViewModels
{
    public class TableVm
    {
        [Required(ErrorMessage = "Название таблицы должно быть установленно.")]
        [Display(Name = "Название таблицы")]
        [RegularExpression(@"[a-zA-Z0-9]{3,40}", ErrorMessage = "Некорректное имя таблицы")]
        public string TableName { get; set; }

        public List<SelectListItem> AvailableColumns = new List<SelectListItem>();
    }
}