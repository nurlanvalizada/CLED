using System;
using System.Collections.Generic;

namespace CledAcademy.Core.Domain
{
    public class Student : Entity
    {
        public string Phone { get; set; }
        public DateTime DateOfBirth { get; set; }
        public double Balance { get; set; }

        public int PersonId { get; set; }
        public Person Person { get; set; }

        public ICollection<StudentLesson> StudentLessons { get; set; }
        public ICollection<ShoppingCard> ShoppingCards { get; set; }
        public ICollection<AccountTopUp> AccountTopUps { get; set; }
        public ICollection<StudentOrder> StudentOrders { get; set; }
        public ICollection<StudentTestAnswer> StudentTestAnswers { get; set; }
        public ICollection<StudentOpenTestAnswer> StudentOpenTestAnswers { get; set; }
    }
}