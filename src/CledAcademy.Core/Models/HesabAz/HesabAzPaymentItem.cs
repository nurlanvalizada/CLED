namespace CledAcademy.Core.Models.HesabAz
{
    public class HesabAzPaymentItem
    {
        public string MerchantName { get; set; }
        public int Amount { get; set; }
        public string Lang { get; set; }
        public string CardType { get; set; }
        public string Description { get; set; }
        public string HashCode { get; set; }
    }
}
