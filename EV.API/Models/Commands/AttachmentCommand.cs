namespace EnVietSocialNetWorkAPI.Model.Commands
{
    public class CreateAttachmentListCommand
    {
        public List<CreateAttachmentCommand> Attachments { get; set; } = new List<CreateAttachmentCommand>();
    }

    public class CreateAttachmentCommand
    {
        public string Media { get; set; }
        public string? Description { get; set; }
    }
}
