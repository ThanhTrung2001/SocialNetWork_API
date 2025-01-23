using EV.Common.Services.Email.Model;

namespace EV.Common.Services.Email
{
    public interface IEmailHandler
    {
        void SendEmail(EmailMessage message);
    }
}
