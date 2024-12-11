using System.Collections.Generic;

namespace CledAcademy.Core.Domain
{
    public class Lesson : Entity
    {
        public string Name { get; set; }
        public string VideoUrl { get; set; }
        public double Price { get; set; }
        public bool Status { get; set; }

        public int SectionId { get; set; }
        public Section Section { get; set; }

        public ICollection<StudentLesson> StudentLessons { get; set; }
    }
}