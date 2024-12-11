using System.Collections.Generic;

namespace CledAcademy.Core.Domain
{
    public class Section : Entity
    {
        public string Name { get; set; }
        public bool Status { get; set; }
        public string Description { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }

        public ICollection<Lesson> Lessons { get; set; }
    }
}