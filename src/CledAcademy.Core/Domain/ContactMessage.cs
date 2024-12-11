using System;

namespace CledAcademy.Core.Domain
{
    public class ContactMessage : Entity
    {
        public string FullName { get; set; }
        public string Subject { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Message { get; set; }
        public DateTime SendDate { get; set; }
    }
}