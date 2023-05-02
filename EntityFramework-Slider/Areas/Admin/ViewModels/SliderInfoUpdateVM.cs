using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework_Slider.Areas.Admin.ViewModels
{
    public class SliderInfoUpdateVM 
    {
        [Required(ErrorMessage = "Don`t be empty")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Don`t be empty")]
        public string Description { get; set; }

        public string SignatureImage { get; set; }
        public IFormFile Photo { get; set; }
    }
}
