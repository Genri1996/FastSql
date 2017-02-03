using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CursachPrototype.ViewModels
{
    public class CreateAnonymouslyDbVm
    {
        [HiddenInput(DisplayValue = false)]
        public List<string> AvailableServers { get; set; }

        [Required(ErrorMessage = "Поле должно быть выбрано.")]
        [Display(Name = "Тип СУБД")]
        public string SelectedServer { get; set; }

        [Required(ErrorMessage = "Название БД должно быть установленно.")]
        [Display(Name = "Название")]
        [RegularExpression(@"[A-z]{3,40}", ErrorMessage = "Некорректное имя БД")]
        public string DataBaseName { get; set; }

        [Range(1, 72, ErrorMessage = "От 1 до 72 часа")]
        public int? StoreHours{ get; set; }
        
           
    }
}