namespace EnVietSocialNetWorkAPI.Entities.Commands
{
    public class NewMessage
    {
        public string Content { get; set; }
        public Guid UserId { get; set; }
        //public Guid ChatboxId { get; set; }
        //public ICollection<MessageReact>? Reacts { get; set; }
    }
}
