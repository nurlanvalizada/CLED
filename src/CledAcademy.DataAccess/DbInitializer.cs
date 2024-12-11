using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CledAcademy.Core.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace CledAcademy.DataAccess
{
    public class DbInitializer
    {
        public static async Task Initialize(ApplicationDbContext context, RoleManager<IdentityRole> roleManager)
        {
            await context.Database.EnsureCreatedAsync();

            if (context.Teachers.Any())
                return;

            #region adding SuperAdmin and Admin Roles

            await roleManager.CreateAsync(new IdentityRole {Name = "SuperAdmin"});
            await roleManager.CreateAsync(new IdentityRole {Name = "Admin"});

            #endregion

            var teachers = new List<Teacher>
            {
                new Teacher
                {
                    FullName = "Samir Əliyev",
                    About = "Samir Əliyev 1990-cı ilin 1 yanvarında Bakı şəhərində anadan olub.",
                    Profession = "Məntiq müəllimi",
                    ImageUrl = "/images/teachers/teacher1.jpg",
                    Courses = new List<Course>
                    {
                        new Course
                        {
                            Name = "Məntiq",
                            Status = true,
                            Description =
                                "Məntiq magistratura qəbul imtahanlarının 1-ci turunun əsas fənnidir və 50 sualdan ibarətdir. Burada bakalavrların məntiqi təfəkkür qabiliyyətlərini nümayiş etdirirlər. Kurs qəbul imtahanı proqramını tam əhatə edir və real testlərin izahı üzərində videolardan ibarətdir",
                            ImageUrl = "/images/courses/logic.jpg",
                            VideoUrl = "your video url"
                        }
                    }
                },
                new Teacher
                {
                    FullName = "Samirə Əliyeva",
                    About = "Samirə Əliyeva 1990-cı ilin 1 yanvarında Bakı şəhərində anadan olub.",
                    Profession = "İnformatika müəllimi",
                    ImageUrl = "/images/teachers/teacher2.jpg",
                    Courses = new List<Course>
                    {
                        new Course
                        {
                            Name = "İnformatika",
                            Description =
                                "İnformatika magistratura qəbul imtahanlarının 1-ci turunun digər bir fənnidir və 25 sualdan ibarətdir. Burada bakalavrlar öz texniki və kompyuter bilikərini nümayiş etdirirlər. İnformatika kursu qəbul imtahanı proqramını tam əhatə edir və real testlərin izahı üzərində videolardan ibarətdir",
                            Status = true,
                            ImageUrl = "/images/courses/information.jpg",
                            VideoUrl = "your video url"
                        }
                    }
                },
                new Teacher
                {
                    FullName = "Cavanşir Əliyev",
                    About = "Cavanşir Əliyev 1990-cı ilin 1 yanvarında Bakı şəhərində anadan olub.",
                    Profession = "İngilis dili müəllimi",
                    ImageUrl = "/images/teachers/teacher3.jpg",
                    Courses = new List<Course>
                    {
                        new Course
                        {
                            Name = "İngilis dili",
                            ImageUrl = "/images/courses/english.jpg",
                            Description =
                                "İngilis dili magistratura qəbul imtahanlarının 1-ci turunun xarici dil fənnlərindən biridir və 25 sualdan ibarətdir. Burada bakalavrlar öz ingilis dili bilikərini nümayiş etdirirlər. İngilis dili kursu qəbul imtahanı proqramını tam əhatə edir və real testlərin izahı üzərində videolardan ibarətdir",
                            Status = true,
                            VideoUrl = "your video url"
                        }
                    }
                }
            };
            context.Teachers.AddRange(teachers);
            await context.SaveChangesAsync();

            var faqs = new List<Faq>
            {
                new Faq
                {
                    Question = "Hər bir video dərsliyə görə nə qədər ödənilməlidir?",
                    Answer =
                        " Hər bir video dərsliyə görə sadəcə 1 manat ödənilir. Siz hər hansı bir dərsin hər hansı bir bölməsində istəyinizə uyğun olan video dərsliyi seçə bilərsiniz. Bunun üçün bütün dərsi və ya bütün bölmə üçün ödəniş etməli deyilsiniz."
                },
                new Faq
                {
                    Question =
                        "Əgər birbaşa bir neçə video dərsliyi bir bölmə kimi və ya tam bir dərs kimi əldə etmək istəsəm, o zaman güzəşt olunacaq mı?",
                    Answer =
                        "Bəli. Əgər bir bölmə və ya bütün dərs olaraq ödəniş etsəniz, 10%-dən başlayaraq güzəştlər olunacaqdır."
                },
                new Faq
                {
                    Question = "Video dərsliklərin köçürülməsinə və ya hardasa yayılmasına icazə var mı?",
                    Answer = "Xeyr. Buna heç bir halda icazə verilmir. Bu bizim şərtlərdə də qeyd olunub."
                },
                new Faq
                {
                    Question =
                        "Bir dəfə ödəniş edildiyi zaman, o vide dərsliyin(lərin) istifadə edilmə müddəti nə qədərdir?",
                    Answer =
                        "Ödəniş edildiyi zamandan etibarən platforma hesabınızın aktiv olduğu qədər və həmən video dərslik yenilənənə qədər siz hesabınıza daxil olub o video dərsliyə baxa bilrəsiniz."
                },
                new Faq
                {
                    Question = "Ödənişi necə edə bilərəm ?",
                    Answer = "Ödənişi öz bank hesabınızla məlumatlarları qeyd etməklə tamamlaya bilərsiniz."
                },
                new Faq
                {
                    Question = "Sizin digər onlayn və real kurslardan fərqiniz nədir ?",
                    Answer =
                        "Hal-hazırda yalnız Azərbaycanda magistraturanın 1-ci hissəsinə qəbul üçün tələb olunan 3 fəndən (Məntiq, İnformatika və İngilis dili) video dərsliklər təklif olunur. Eyni zamanda, video dərsliklər yüksək səviyyəli müəllimlər və peşəkar komanda tərəfindən hazırlanır. Fərqi video dərsliklərimizdə daha yaxşı hiss edəcəksiniz! :)"
                }
            };

            context.Faqs.AddRange(faqs);
            await context.SaveChangesAsync();
        }
    }
}