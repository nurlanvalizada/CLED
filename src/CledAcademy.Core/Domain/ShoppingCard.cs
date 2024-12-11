using CledAcademy.Core.Models;

namespace CledAcademy.Core.Domain
{
    public class ShoppingCard : Entity
    {
        public int StudentId { get; set; }
        public Student Student { get; set; }

        public OrderType OrderType { get; set; }
        public int OrderId { get; set; }
    }
}