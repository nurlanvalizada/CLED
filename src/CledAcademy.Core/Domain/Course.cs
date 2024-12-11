using System.Collections.Generic;

namespace CledAcademy.Core.Domain
{
    public class Course : Entity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string VideoUrl { get; set; }
        public bool Status { get; set; }
       
        public int? TeacherId { get; set; }
        public Teacher Teacher { get; set; }

        public ICollection<Section> Sections { get; set; }
        public ICollection<Test> Tests { get; set; }
    }
}