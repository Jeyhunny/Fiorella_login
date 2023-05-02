using System.ComponentModel.DataAnnotations;

namespace EntityFramework_Slider.ViewModels.Account
{
    public class RegisterVM
    {
        [Required]
        public string FullName { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.EmailAddress, ErrorMessage ="E-maail is not valid")]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)] // typeni qeyd edirikki password olsun
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password),Compare(nameof(Password))] //typeni qeyd edirikki password olsun, hemde passwordla eyni olub olmamasini yoxlayiriq
        public string ComfirmPassword { get; set; }




    }
}
