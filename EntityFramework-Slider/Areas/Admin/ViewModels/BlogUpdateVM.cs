using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework_Slider.Areas.Admin.ViewModels
{
    public class BlogUpdateVM
    {
        [Required(ErrorMessage = "Don`t be empty")]
        public string Header { get; set; }


        [Required(ErrorMessage = "Don`t be empty")]
        public string Description { get; set; }


        public string Image { get; set; }


        [Required(ErrorMessage = "Don`t be empty")]
        public DateTime Date { get; set; }

        public IFormFile Photo { get; set; }
    }
}
