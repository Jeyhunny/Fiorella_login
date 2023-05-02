using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework_Slider.Models
{
    public class Experts: BaseEntity
    {
        [Required(ErrorMessage = "Don't be empty")]
        public string Name { get; set; }

       
        public string Image { get; set; }

        [Required(ErrorMessage = "Don't be empty")]
        public string Position { get; set; }

        [NotMapped,Required(ErrorMessage = "Don't be empty")]
        public IFormFile Photo { get; set; }

    }
}
