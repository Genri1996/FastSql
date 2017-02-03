using System.ComponentModel.DataAnnotations;

namespace CursachPrototype.ViewModels
{
    public class AnonymousDbVm : DbVm
    {
        [Range(1, 72, ErrorMessage = "От 1 до 72 часа")]
        public int StoreHours{ get; set; }   
    }
}