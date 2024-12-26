namespace EnVietSocialNetWorkAPI.Entities.Commands
{
    public class NewComment
    {
        public string Content { get; set; }
        public string MediaURL { get; set; }
        public Guid UserId { get; set; }
    }
}
