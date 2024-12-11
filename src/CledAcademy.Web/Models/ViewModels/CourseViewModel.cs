using CledAcademy.Core.Domain;

namespace CledAcademy.Web.Models.ViewModels
{
    public class CourseViewModel
    {
        public Course Course { get; set; }
        public bool IsBoughtAnyLesson { get; set; }
        public double Price { get; set; }
    }
}