using System;
using CledAcademy.Core.Models;

namespace CledAcademy.Core.Domain
{
    public class StudentOrder : Entity
    {
        public int StudentId { get; set; }
        public Student Student { get; set; }

        public int OrderId { get; set; }
        public OrderType OrderType { get; set; }

        public DateTime DateTime { get; set; }
        public double Fee { get; set; }
    }
}