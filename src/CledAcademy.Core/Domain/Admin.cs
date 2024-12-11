using System.Collections.Generic;

namespace CledAcademy.Core.Domain
{
    public class Admin : Entity
    {
        public int PersonId { get; set; }
        public Person Person { get; set; }

        public ICollection<News> Newses { get; set; }
    }
}