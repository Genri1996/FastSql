using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CursachPrototype.ViewModels
{
    public class CreateColumnVm
    {
        [Required(ErrorMessage = "Название колонки должно быть установленно.")]
        [Display(Name = "Название")]
        [RegularExpression(@"[a-zA-Z0-9]{3,40}", ErrorMessage = "Некорректное имя колонки")]
        public string ColumnName { get; set; }

        [Required(ErrorMessage = "Поле должно быть выбрано.")]
        public string TypeName { get; set; }

        [Range(1, 5000, ErrorMessage = "От 1 до 72 часа")]
        public int TypeLength { get; set; }
        public string Constraints { get; set; }

        public IEnumerable<SelectListItem> AvailableTypes => 
            new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Строка", Value = "String", Selected = true
                },
                new SelectListItem
                {
                    Text = "Целое", Value = "Integer"
                },
                new SelectListItem
                {
                    Text = "Дробное", Value = "Double"
                },
                new SelectListItem
                {
                    Text = "Дата и время", Value = "DateTime"
                }

            };
    }
}