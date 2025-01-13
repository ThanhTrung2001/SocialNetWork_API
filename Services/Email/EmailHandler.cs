using EnVietSocialNetWorkAPI.Services.Email.Model;
using System.Net;
using System.Net.Mail;


namespace EnVietSocialNetWorkAPI.Services.Email
{
    public class EmailHandler : IEmailHandler
    {
        private readonly EmailConfiguration _emailConfig;
        public EmailHandler(EmailConfiguration emailConfig)
        {
            _emailConfig = emailConfig;
        }

        public void SendEmailAsync(EmailMessage email)
        {
            var message = new MailMessage()
            {
                From = new MailAddress(_emailConfig.From),
                Subject = email.Subject,
                IsBodyHtml = true,
                Body = email.Body,
            };
            foreach (var item in email.ToEmails)
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
