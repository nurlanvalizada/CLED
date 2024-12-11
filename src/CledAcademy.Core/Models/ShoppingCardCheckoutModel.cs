using CledAcademy.Core.Domain;

namespace CledAcademy.Core.Models
{
    public class ShoppingCardCheckoutModel
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int OrderId { get; set; }
        public OrderType OrderType { get; set; }

        public string Name { get; set; }
        public double Price { get; set; }
        public Lesson Lesson { get; set; }
        public Course Course { get; set; }
        public Section Section { get; set; }
    }
}