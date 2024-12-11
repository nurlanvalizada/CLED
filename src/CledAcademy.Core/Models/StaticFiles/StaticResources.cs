using System;
using System.Collections.Generic;

namespace CledAcademy.Core.Models.StaticFiles
{
    public static class StaticResources
    {
        public static Dictionary<int, string> Months = new Dictionary<int, string>
        {
            {1, "Yanvar"},
            {2, "Fevral"},
            {3, "Mart"},
            {4, "Aprel"},
            {5, "May"},
            {6, "İyun"},
            {7, "İyul"},
            {8, "Avqust"},
            {9, "Sentyabr"},
            {10, "Oktyabr"},
            {11, "Noyabr"},
            {12, "Dekabr"}
        };

        public static Dictionary<string, Tuple<string, string>> CheckoutResults = new Dictionary<string, Tuple<string, string>>
        {
            {
                "InSufficentBalance",
                new Tuple<string, string>("Sizin balansinızda bu əməliyyatı yerinə yetirmək üçün kifayət qədər məbləğ yoxdur.",
                    "error")
            },
            {"Success", new Tuple<string, string>("Sizin əməliyyat uğurla yerinə yetirilmişdir. Təşəkkür edirik.", "success")},
            {
                "AlreadyBoughtCourse",
                new Tuple<string, string>(
                    "Siz bu kursun dərslərinin hamısını və ya bir hissəni almısınız. Uyğun bölməni və ya dərsi almağa çalışın",
                    "warning")
            },
            {
                "AlreadyBoughtSection",
                new Tuple<string, string>(
                    "Siz bu bölmənin dərslərinin hamısını və ya bir hissəni almısınız. Uyğun dərsi almağa çalışın", "warning")
            },
            {
                "AlreadyBoughtLesson",
                new Tuple<string, string>("Siz bu dərsi artıq almısınız. Almadığınız dərsləri almağa çalışın", "warning")
            },
            {
                "AlreadyAddedShoppingCardCourse",
                new Tuple<string, string>(
                    "Siz bu kursu və ya onun bölmələrindən(dərslərindən) hər hansı birini artıq səbətə əlavə etmisiniz", "warning")
            },
            {
                "AlreadyAddedShoppingCardSection",
                new Tuple<string, string>(
                    "Siz bu bölməni, bölmənin aid olduğu kursu və ya onun dərslərindən hər hansı birini artıq səbətə əlavə etmisiniz",
                    "warning")
            },
            {
                "AlreadyAddedShoppingCardLesson",
                new Tuple<string, string>("Siz bu dəsri, dərsin aid olduğu bölməni və ya kursu artıq səbətə əlavə etmisiniz",
                    "warning")
            },
            {
                "Failure",
                new Tuple<string, string>("Sizin əməliyyatınız zamanı xəta baş verdi. Zəhmət olmasa yenidən cəhd edin.", "error")
            }
        };

        public static string GetIdentityMessage(string key, string username, string email)
        {
            string description = null;
            switch (key)
            {
                case "PasswordRequiresLower":
                    description = "Şifrədə ən azı 1 kiçik hərf olmalıdır.";
                    break;
                case "PasswordRequiresDigit":
                    description = "Şifrədə ən azı 1 rəqəm olmalıdır.";
                    break;
                case "PasswordTooShort":
                    description = "Şifrədə ən azı 5 simvoldan ibarət olmalıdır.";
                    break;
                case "DuplicateEmail":
                    description = $"'{email}' email ünvanı artıq istifadədədir.";
                    break;
                case "DuplicateUserName":
                    description = $"'{username}' istifadəçi adı artıq istifadədədir.";
                    break;
                case "InvalidEmail":
                    description = $"'{email}' düzgün email ünvanı deyil.";
                    break;
                case "InvalidUserName":
                    description = $"'{username}' düzgün istifadəçi adı deyil.";
                    break;
            }
            return description;
        }
    }
}