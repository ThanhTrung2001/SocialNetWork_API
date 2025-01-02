namespace EnVietSocialNetWorkAPI.Entities.Models.SocialNetwork.Chat
{
    public class Message : BaseClass
    {
        public string Content { get; set; }
        public bool IsPinned { get; set; }
        public Guid UserId { get; set; }
        public Guid ChatboxId { get; set; }
        public virtual User User { get; set; }
        public virtual ChatBox ChatBox { get; set; }
        //public ICollection<MessageReact>? Reacts { get; set; }
    }
}
