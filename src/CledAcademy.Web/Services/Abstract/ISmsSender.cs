using System.Threading.Tasks;

namespace CledAcademy.Web.Services.Abstract
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}
