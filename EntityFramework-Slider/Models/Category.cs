

using System.ComponentModel.DataAnnotations;

namespace EntityFramework_Slider.Models
{
    public class Category:BaseEntity
    {
        [Required(ErrorMessage = "Don't be empty")] //null olub olmamasini yoxlayir. bu atributu qoyuruqsa null gele bilmez. yeni category create edende name vacib yazilmalidir.
        public string Name { get; set; }
        public ICollection<Product> Products { get; set; }  //categoryden yola chixanda producta chata bilmek uchun
    }
}
