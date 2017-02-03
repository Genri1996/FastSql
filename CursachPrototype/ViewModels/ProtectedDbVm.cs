using System.ComponentModel.DataAnnotations;

namespace CursachPrototype.ViewModels
{
    public class ProtectedDbVm : DbVm
    {
        [Display(Name = "Пароль к базе данных")]
        public string DataBasePassword { get; set; }

        public override bool IsPublic => false;
    }
}