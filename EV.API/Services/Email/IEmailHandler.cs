using EnVietSocialNetWorkAPI.Services.Email.Model;

namespace EnVietSocialNetWorkAPI.Services.Email
{
    public interface IEmailHandler
    {
        void SendEmail(EmailMessage email);
    }
}
