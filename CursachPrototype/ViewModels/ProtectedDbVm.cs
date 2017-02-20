using System.ComponentModel.DataAnnotations;

namespace CursachPrototype.ViewModels
{
    public class ProtectedDbVm : DbVm
    {
        [Required]
        [Display(Name = "Пароль к базе данных")]
        public string DataBasePassword { get; set; }

        [Required]
        [Compare("DataBasePassword", ErrorMessage = "Пароли не совпадают")]
        public string PasswordConfiramtion { get; set; }

        public override bool IsPublic => false;
    }
}