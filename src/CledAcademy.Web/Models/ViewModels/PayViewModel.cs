using System.ComponentModel.DataAnnotations;

namespace CledAcademy.Web.Models.ViewModels
{
    public class PayViewModel
    {
        [Required(ErrorMessage = "Məbləği daxil edin")]
        [RegularExpression(@"^\d*\,?\d*$", ErrorMessage = "Məbləğ düzgün daxil edilməyib. Düzgün format 00,00")]
        public string Amount { get; set; }

        [RegularExpression(@"^v|m$", ErrorMessage = "Kartın növünü seçin")]
        public string CardType { get; set; }
    }
}