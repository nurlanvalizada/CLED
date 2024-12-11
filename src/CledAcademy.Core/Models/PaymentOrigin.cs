namespace CledAcademy.Core.Models
{
    public enum PaymentOrigin : byte
    {
        HesabAz = 0,
        Cash = 1,
        Sms = 2,
        Million = 3,
        CledAcademy = 4,
        CledAcademyPaymentBonus = 5,
        CledAcademyFriendBonus = 6
    }
}