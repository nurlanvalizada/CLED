using System.ComponentModel.DataAnnotations;

namespace CledAcademy.Web.Models.ViewModels.AccountViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "{0} sahəsini doldurmaq tələb olunur")]
        [EmailAddress(ErrorMessage = "{0} sahəsi düzgün daxil edilməyib")]
        public string Email { get; set; }
    }
}
