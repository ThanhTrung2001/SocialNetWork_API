using EnVietSocialNetWorkAPI.Services.Email.Model;

namespace EnVietSocialNetWorkAPI.Services.Email
{
    public interface IEmailHandler
    {
        void SendEmailAsync(EmailMessage email);
    }
}
