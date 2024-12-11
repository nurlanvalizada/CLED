using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CledAcademy.Web.Services.Abstract;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace CledAcademy.Web.Services.Concret
{
    public class AuthMessageSender : IEmailSender, ISmsSender
    {
        public Task SendEmailAsync(string email, string title, string subTitle, string body)
        {
            var emailTemplate = GetDefaultTemplate();
            emailTemplate = emailTemplate.Replace("{EMAIL_TITLE}", title)
                .Replace("{EMAIL_SUB_TITLE}", subTitle)
                .Replace("{EMAIL_BODY}", body);

            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("CLED Academy", "info@cledacademy.com"));
            emailMessage.To.Add(new MailboxAddress(email));
            emailMessage.Subject = title;

            var bodyBuilder = new BodyBuilder { HtmlBody = emailTemplate };
            emailMessage.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                int yourPort = 587;
                client.Connect("smtp host", yourPort, SecureSocketOptions.StartTls);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate("smtp user", "smtp password");
                client.Send(emailMessage);
                client.Disconnect(true);
            }
            return Task.FromResult(0);
        }

        private string GetDefaultTemplate()
        {
            var assembly = typeof(AuthMessageSender).GetTypeInfo().Assembly;
            using (var stream = assembly.GetManifestResourceStream("CledAcademy.Web.Services.Concret.default.html"))
            {
                var bytes = new byte[stream.Length];
                stream.Read(bytes, 0, (int) stream.Length);
                return Encoding.UTF8.GetString(bytes, 3, bytes.Length - 3);
            }
        }

        public Task SendSmsAsync(string number, string message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }
}
