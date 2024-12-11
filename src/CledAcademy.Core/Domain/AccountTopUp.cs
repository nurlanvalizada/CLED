using System;
using CledAcademy.Core.Models;

namespace CledAcademy.Core.Domain
{
    public class AccountTopUp : Entity
    {
        public int StudentId { get; set; }
        public Student Student { get; set; }

        public DateTime DateTime { get; set; }
        public PaymentOrigin PaymetOrigin { get; set; }
        public double Amount { get; set; }

        public string StatusCode { get; set; }
        public string CardNumber { get; set; }
        public string PaymentKey { get; set; }
        public string Rrn { get; set; }
    }
}