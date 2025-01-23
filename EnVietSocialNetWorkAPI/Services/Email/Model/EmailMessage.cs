namespace EnVietSocialNetWorkAPI.Services.Email.Model
{
    public class EmailMessage
    {
        public List<string> ToEmails { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
