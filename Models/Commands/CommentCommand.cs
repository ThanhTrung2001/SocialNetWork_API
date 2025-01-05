namespace EnVietSocialNetWorkAPI.Models.Commands
{
    public class CreateCommentCommand
    {
        public string Content { get; set; }
        public Guid UserId { get; set; }
    }

    public class CreateResponseCommentCommand
    {
        public string Content { get; set; }
        public Guid UserId { get; set; }
        public Guid PostId { get; set; }
    }

    public class EditCommentCommand
    {
        public string Content { get; set; }
        public Guid UserId { get; set; }
    }
}
