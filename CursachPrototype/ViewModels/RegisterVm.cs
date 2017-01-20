using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace CursachPrototype.ViewModels
{
    public class RegisterVm
    {
        [Required]
        public string Email { get; set; }

        [Required]
        [RegularExpression("[A-z]{3,10}", ErrorMessage = "Латинские буквы, от 3 до 10 шт")]
        public string NickName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        public string PasswordConfirm { get; set; }
    }
}