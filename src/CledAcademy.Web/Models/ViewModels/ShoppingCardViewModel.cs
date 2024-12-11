using CledAcademy.Core.Models;

namespace CledAcademy.Web.Models.ViewModels
{
    public class ShoppingCardViewModel
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int OrderId { get; set; }
        public OrderType OrderType { get; set; }

        public string Name { get; set; }
        public double Price { get; set; }
    }
}