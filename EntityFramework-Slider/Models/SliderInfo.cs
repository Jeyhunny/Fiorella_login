using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EntityFramework_Slider.Models
{
    public class SliderInfo:BaseEntity
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        public string SignatureImage { get; set; }


        [NotMapped,Required]
        public IFormFile Photo { get; set; }
    }
}
