using System.ComponentModel.DataAnnotations;

namespace CledAcademy.Web.Models.ViewModels
{
    public class ContactMessageViewModel
    {
        [Required(ErrorMessage = "Ad və soyadı daxil etmək tələb olunur")]
        [Display(Name = "Ad və soyad")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email ünvanı daxil etmək tələb olunur")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Email ünvanı düzgün daxil edin")]
        public string Email { get; set; }

        [DataType(DataType.PhoneNumber, ErrorMessage = "Telefon nömrəsini düzgün daxil edin")]
        [Display(Name = "Telefon")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Mövzunu daxil etmək tələb olunur")]
        [StringLength(50, ErrorMessage = "{0} ən azı {2} və ən çoxu {1} simvoldan ibarət olmalıdır.", MinimumLength = 5)]
        [Display(Name = "Mövzu")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Masajı daxil etmək tələb olunur")]
        [StringLength(500, ErrorMessage = "{0} ən azı {2} və ən çoxu {1} simvoldan ibarət olmalıdır.", MinimumLength = 10)]
        [Display(Name = "Mesaj")]
        public string Message { get; set; }
    }
}
