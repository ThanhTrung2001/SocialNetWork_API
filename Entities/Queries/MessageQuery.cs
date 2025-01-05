using EnVietSocialNetWorkAPI.Models.Queries;

namespace EnVietSocialNetWorkAPI.Entities.Queries
{
    public class MessageQuery
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Content { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string AvatarUrl { get; set; }
        public bool IsPinned { get; set; }
        public int ReactCount { get; set; }
    }

    public class MessageDetailQuery
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Content { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string AvatarUrl { get; set; }
        public bool IsPinned { get; set; }
        public List<MessageReactQuery> Reacts { get; set; } = new List<MessageReactQuery>();
    }
}
