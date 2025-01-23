namespace EV.Common.Services.Email.Model
{
    public class EmailMessage
    {
        public List<string> Receivers { get; set; } = new List<string>();
        public string? Subject { get; set; }
        public string? Body { get; set; }
    }
}
