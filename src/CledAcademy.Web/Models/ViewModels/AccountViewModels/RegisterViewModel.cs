using System.ComponentModel.DataAnnotations;
using CledAcademy.Web.Models.CustomAttributes;

namespace CledAcademy.Web.Models.ViewModels.AccountViewModels
{
    public class RegisterViewModel : AccountEditViewModel
    {
        [DataType(DataType.Password)]
        [Display(Name = "Şifrənin Təsdiqi")]
        [Compare("Password", ErrorMessage = "Şifrə və təsdiqləmə şifrəsi uyğun gəlmir")]
        public string ConfirmPassword { get; set; }

        [BooleanRequiredTrue(ErrorMessage = "İstifadəçi Razılaşmasını qəbul etməlisiniz")]
        public bool IsAcceptedTermsAndConditions { get; set; }
    }
}
