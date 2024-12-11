using System;

namespace CledAcademy.Core.Domain
{
    public class StudentLesson : Entity
    {
        public DateTime StartDate { get; set; }
        public DateTime? LastVisitDateTime { get; set; }
        public int VisitCounts { get; set; }

        public int StudentId { get; set; }
        public Student Student { get; set; }

        public int LessonId { get; set; }
        public Lesson Lesson { get; set; }
    }
}