using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CursachPrototype.ViewModels
{
    public class ServerSelectionVm
    {
        [HiddenInput(DisplayValue = false)]
        public List<string>  AvailableServers { get; set; }

        [Required(ErrorMessage = "Поле должно быть выбрано.")]
        [Display(Name = "Тип СУБД")]
        public string SelectedServer { get; set; }

        [Required(ErrorMessage = "Название БД должно быть установленно.")]
        [Display(Name = "Название")]
        [RegularExpression(@"[A-z]{3,40}", ErrorMessage = "Некорректное имя БД")]
        public string DataBaseName { get; set; }

        [Display(Name = "Публичная база данных")]
        public bool IsDataBasePublic { get; set; } = true;

        [Display(Name = "Пароль к базе данных")]
        public string DataBasePassword { get; set; }
    }
}