using System.ComponentModel.DataAnnotations;

namespace CledAcademy.Web.Models.ViewModels.AccountViewModels
{
    public class SuggestViewModel
    {
        [Required(ErrorMessage = "{0} sahəsini doldurmaq tələb olunur")]
        [EmailAddress(ErrorMessage = "{0} sahəsi düzgün daxil edilməyib")]
        [Display(Name = "Dostunuzun Email Ünvanı")]
        public string Email { get; set; }
    }
}