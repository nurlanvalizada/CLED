using System.Collections.Generic;

namespace CledAcademy.Core.Domain
{
    public class Answer : Entity
    {
        public string Text { get; set; }
        public bool Status { get; set; }

        public int TestId { get; set; }
        public Test Test { get; set; }

        public TestAnswer TestAnswer { get; set; }

        public ICollection<StudentTestAnswer> StudentTestAnswers { get; set; }
    }
}