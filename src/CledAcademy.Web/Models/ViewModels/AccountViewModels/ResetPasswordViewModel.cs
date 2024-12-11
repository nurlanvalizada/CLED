using System.ComponentModel.DataAnnotations;

namespace CledAcademy.Web.Models.ViewModels.AccountViewModels
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "{0} sahəsini doldurmaq tələb olunur")]
        [EmailAddress(ErrorMessage = "{0} sahəsi düzgün daxil edilməyib")]
        [Display(Name = "Email ünvanı")]
        public string Email { get; set; }

        [Required(ErrorMessage = "{0} sahəsini doldurmaq tələb olunur")]
        [StringLength(30, ErrorMessage = "{0} ən azı {2} və ən çoxu {1} simvoldan ibarət olmalıdır", MinimumLength = 6)]
        [Display(Name = "Yeni Şifrə")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Şifrənin Təsdiqi")]
        [Compare("Password", ErrorMessage = "Şifrə və təsdiqləmə şifrəsi uyğun gəlmir")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }
}
