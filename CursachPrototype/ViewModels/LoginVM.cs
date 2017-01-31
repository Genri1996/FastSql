using System.ComponentModel.DataAnnotations;

namespace CursachPrototype.ViewModels
{
    public class LoginVm
    {
        [Required]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}