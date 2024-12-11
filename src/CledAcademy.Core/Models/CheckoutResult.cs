namespace CledAcademy.Core.Models
{
    public enum CheckoutResult
    {
        InSufficentBalance = 0,
        Success = 1,
        AlreadyBoughtLesson = 2,
        AlreadyBoughtSection = 3,
        AlreadyBoughtCourse = 4,
        AlreadyAddedShoppingCardCourse = 5,
        AlreadyAddedShoppingCardSection = 6,
        AlreadyAddedShoppingCardLesson = 7,
        Failure = 5
    }
}