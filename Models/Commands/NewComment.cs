namespace EnVietSocialNetWorkAPI.Entities.Commands
{
    public class CreateCommentCCommand
    {
        public string Content { get; set; }
        public Guid UserId { get; set; }
    }
}
