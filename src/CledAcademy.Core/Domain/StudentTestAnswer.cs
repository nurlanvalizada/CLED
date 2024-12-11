namespace CledAcademy.Core.Domain
{
    public class StudentTestAnswer : Entity
    {
        public int StudentId { get; set; }
        public Student Student { get; set; }

        public int AnswerId { get; set; }
        public Answer Answer { get; set; }
    }
}