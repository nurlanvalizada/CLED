using System.ComponentModel.DataAnnotations;

namespace CledAcademy.Web.Models.ViewModels.AccountViewModels
{
    public class AccountEditViewModel
    {
        [Required(ErrorMessage = "{0} sahəsini doldurmaq tələb olunur")]
        [EmailAddress(ErrorMessage = "{0} sahəsi düzgün daxil edilməyib")]
        [Display(Name = "Email ünvanı")]
        public string Email { get; set; }

        [Required(ErrorMessage = "{0} sahəsini doldurmaq tələb olunur")]
        [Display(Name = "İstifadəçi Adı")]
        public string Username { get; set; }

        [Required(ErrorMessage = "{0} sahəsini doldurmaq tələb olunur")]
        [Display(Name = "Ad")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "{0} sahəsini doldurmaq tələb olunur")]
        [Display(Name = "Soyad")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "{0}-i seçmək tələb olunur")]
        [Range(1, 2, ErrorMessage = "{0}-i düzgün seçməmisiniz")]
        [Display(Name = "Cins")]
        public byte Gender { get; set; }

        [Range(1, 31, ErrorMessage = "Günü düzgün seçməmisiniz")]
        public int Day { get; set; }

        [Range(1, 12, ErrorMessage = "Ayı düzgün seçməmisiniz")]
        public int Month { get; set; }

        [Range(1930, 2020, ErrorMessage = "İli düzgün seçməmisiniz")]
        public int Year { get; set; }

        [Display(Name = "Telefon")]
        [RegularExpression(@"\d{10}",ErrorMessage = "Telefon 0501234567 formatda daxil edin")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "{0} sahəsini doldurmaq tələb olunur")]
        [StringLength(30, ErrorMessage = "{0} ən azı {2} və ən çoxu {1} simvoldan ibarət olmalıdır", MinimumLength = 5)]
        [DataType(DataType.Password)]
        [Display(Name = "Şifrə")]
        public string Password { get; set; }
    }
}