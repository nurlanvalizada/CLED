using System.ComponentModel.DataAnnotations;

namespace CledAcademy.Web.Models.ViewModels.AccountViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "{0} sahəsini doldurmaq tələb olunur")]
        [Display(Name = "İstifadəçi Adı")]
        public string Username { get; set; }

        [Required(ErrorMessage = "{0} sahəsini doldurmaq tələb olunur")]
        [DataType(DataType.Password)]
        [Display(Name = "Şifrə")]
        public string Password { get; set; }

        [Display(Name = "Məni Xatırla ?")]
        public bool RememberMe { get; set; }
    }
}
