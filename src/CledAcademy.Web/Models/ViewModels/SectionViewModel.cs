using CledAcademy.Core.Domain;

namespace CledAcademy.Web.Models.ViewModels
{
    public class SectionViewModel
    {
        public Section Section { get; set; }
        public bool IsBoughtAnyLesson { get; set; }
        public double Price { get; set; }
        public int LessonCount { get; set; }
    }
}