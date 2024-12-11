using System.Threading.Tasks;

namespace CledAcademy.Web.Services.Abstract
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string title, string subTitle, string body);
    }
}