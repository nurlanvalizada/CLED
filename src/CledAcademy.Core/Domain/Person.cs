using CledAcademy.Core.Models;

namespace CledAcademy.Core.Domain
{
    public class Person : Entity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public byte[] Image { get; set; }
        public string ImageContentType { get; set; }
        public Gender Gender { get; set; }

        public ApplicationUser ApplicationUser { get; set; }
        public Student Student { get; set; }
        public Admin Admin { get; set; }
    }
}