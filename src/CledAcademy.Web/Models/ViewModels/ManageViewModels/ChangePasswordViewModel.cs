using System.ComponentModel.DataAnnotations;

namespace CledAcademy.Web.Models.ViewModels.ManageViewModels
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "{0} sahəsini doldurmaq tələb olunur")]
        [Display(Name = "Cari Şifrə")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "{0} sahəsini doldurmaq tələb olunur")]
        [StringLength(30, ErrorMessage = "{0} ən azı {2} və ən çoxu {1} simvoldan ibarət olmalıdır", MinimumLength = 6)]
        [Display(Name = "Yeni Şifrə")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Şifrənin Təsdiqi")]
        [Compare("NewPassword", ErrorMessage = "Şifrə və təsdiqləmə şifrəsi uyğun gəlmir")]
        public string ConfirmPassword { get; set; }
    }
}
