

using Microsoft.Extensions.Options;
using MimeKit;

using System.Net;
using System.Net.Mail;
using WebBanThu.Interface;
using WebBanThu.Models;


namespace WebBanThu.Helpers
{
    public class EmailService : IEmailService

    {
        private SmtpSettings _smtpSettings;

        public EmailService(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
        }

        public void SendPaymentConfirmationEmail(string toEmail, string subject, string body, List<string> productImages)
        {
            var model = new PaymentConfirmationModel
            {
                Subject = subject,
                Body = body,
                ProductImages = productImages
            };

            var templatePath = Path.Combine("Views", "EmailTemplates", "PaymentConfirmationEmail.cshtml");
            var emailHtml = File.ReadAllText(templatePath);

            using (var client = new SmtpClient(_smtpSettings.SmtpServer))
            {
                client.Port = _smtpSettings.Port;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(_smtpSettings.Email, _smtpSettings.Password);
                client.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_smtpSettings.Email),
                    Subject = subject,
                    Body = emailHtml,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                client.Send(mailMessage);
            }
        }
    }
    
}
