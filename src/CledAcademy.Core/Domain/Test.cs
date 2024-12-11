using System.Collections.Generic;
using CledAcademy.Core.Models;

namespace CledAcademy.Core.Domain
{
    public class Test : Entity
    {
        public string Content { get; set; }
        public bool Status { get; set; }
        public TestType TestType { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }

        public TestAnswer TestAnswer { get; set; }
        public ICollection<Answer> Answers { get; set; }
    }
}