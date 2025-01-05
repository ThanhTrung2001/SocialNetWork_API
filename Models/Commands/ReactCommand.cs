namespace EnVietSocialNetWorkAPI.Models.Commands
{
    public class CreatePostReactCommand
    {
        public Guid PostId { get; set; }
        public int ReactType { get; set; }
        public Guid UserId { get; set; }
    }

    public class CreateCommentReactCommand
    {
        public Guid CommentId { get; set; }
        public int ReactType { get; set; }
        public Guid UserId { get; set; }
    }

    public class CreateMessageReactCommand
    {
        public Guid MessageId { get; set; }
        public int ReactType { get; set; }
        public Guid UserId { get; set; }
    }

    public class EditReactCommand
    {
        public int ReactType { get; set; }
        public Guid UserId { get; set; }
    }
}
