namespace CledAcademy.Core.Models
{
    public class BonusCalculator
    {
        public static double GetBonusAmount(double originalAmount)
        {
            if (originalAmount < 5)
                return 0;
            if (originalAmount >= 5 && originalAmount < 10)
                return 2;
            if (originalAmount >= 10 && originalAmount < 20)
                return 5;
            if (originalAmount >= 20)
                return originalAmount / 20 * 12;
            return 0;
        }
    }
}