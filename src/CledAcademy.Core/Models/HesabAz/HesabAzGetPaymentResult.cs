namespace CledAcademy.Core.Models.HesabAz
{
    public class HesabAzGetPaymentResult
    {
        public HesabAzStatus Status { get; set; }

        public string PaymentKey { get; set; }

        public string MerchantName { get; set; }

        public int Amount { get; set; }

        public int CheckCount { get; set; }

        public string PaymentDate { get; set; }

        public string CardNumber { get; set; }

        public string Language { get; set; }

        public string Description { get; set; }

        public string Rrn { get; set; }

        public override string ToString()
        {
            return "paymentKey=" + PaymentKey + "<br>"
                   + "statuscode=" + Status.Code + "<br>"
                   + "statusemessage=" + Status.Message + "<br>"
                   + "amount=" + Amount + "<br>"
                   + "merchantname=" + MerchantName + "<br>"
                   + "paymentdate=" + PaymentDate + "<br>"
                   + "cardnumber=" + CardNumber + "<br>"
                   + "language=" + Language + "<br>"
                   + "description=" + Description + "<br>"
                   + "rrn=" + Rrn;
        }

    }
}
