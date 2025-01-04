namespace EnVietSocialNetWorkAPI.Entities.Models.SocialNetwork.Chat
{
    public class Message : BaseClass
    {
        public Guid SenderId { get; set; }
        public Guid ChatGroupId { get; set; }
        public string Content { get; set; }
        public bool IsPinned { get; set; }
        public bool IsResponse { get; set; }
        public int ReactCount { get; set; }
    }

    public enum MessageType
    {
        Chat = 1,
        Media
    }
}
