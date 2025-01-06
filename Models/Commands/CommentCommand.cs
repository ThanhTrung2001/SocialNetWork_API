namespace EnVietSocialNetWorkAPI.Models.Commands
{
    public class CreateCommentCommand
    {
        public string Content { get; set; }
        public Guid UserId { get; set; }
        public bool IsResponse { get; set; }
        public Guid? CommentId { get; set; }
        public List<CreateAttachmentCommand>? Attachments { get; set; } = new List<CreateAttachmentCommand>();

    }

    public class EditCommentCommand
    {
        public string Content { get; set; }
        public Guid UserId { get; set; }
    }
}
