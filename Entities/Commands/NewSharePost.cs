namespace EnVietSocialNetWorkAPI.Entities.Commands
{
    public class SharePostCommand
    {
        public Guid UserId { get; set; }
        public string Content { get; set; }
        public string TargetType { get; set; }
        public Guid TargetID { get; set; }
    }
}
