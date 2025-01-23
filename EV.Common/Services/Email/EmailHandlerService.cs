using EV.Common.Services.Email.Model;
using EV.Common.SettingConfigurations;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace EV.Common.Services.Email
{
    public class EmailHandlerService : IEmailHandler
    {
        private readonly EmailConfiguration _emailConfig;
        public EmailHandlerService(IOptions<EmailConfiguration> emailConfig)
        {
            _emailConfig = emailConfig.Value;
        }

        public void SendEmail(EmailMessage email)
        {
            var message = new MailMessage()
            {
                From = new MailAddress(_emailConfig.From),
                Subject = email.Subject,
                IsBodyHtml = true,
                Body = email.Body,
            };
            foreach (var item in email.Receivers)
            {
                message.To.Add(new MailAddress(item));
            }
            var smtp = new SmtpClient(_emailConfig.SmtpServer)
            {
                Port = _emailConfig.Port,
                Credentials = new NetworkCredential(_emailConfig.From, _emailConfig.Password),
                EnableSsl = true,
            };
            try
            {
                smtp.Send(message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
