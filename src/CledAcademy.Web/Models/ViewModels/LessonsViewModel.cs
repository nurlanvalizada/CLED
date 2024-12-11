namespace CledAcademy.Web.Models.ViewModels
{
    public class LessonViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string VideoUrl { get; set; }
        public int SectionId { get; set; }
        public bool IsBought { get; set; }
    }
}