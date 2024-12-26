namespace EnVietSocialNetWorkAPI.Entities.Models.SocialNetwork
{
    public class React : BaseClass
    {
        public ReactType ReactType { get; set; }
        public virtual User User { get; set; }
        public virtual Post Post { get; set; }
    }

    public class CommentReact
    {
        public Guid UserId { get; set; }
        public ReactType ReactType { get; set; }
        public Guid CommentId { get; set; }
    }

    public class MessageReact : BaseClass
    {
        public Guid UserId { get; set; }
        public Guid MessageId { get; set; }
        public ReactType ReactType { get; set; }
    }

    public enum ReactType
    {
        Like = 1,
        Heart = 2,
        Smail = 3,
    }
}
