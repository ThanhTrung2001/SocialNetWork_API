using EnVietSocialNetWorkAPI.Models.Queries;

namespace EnVietSocialNetWorkAPI.Entities.Queries
{
    public class CommentQuery
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public string MediaURL { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string AvatarUrl { get; set; }
        public int ReactCount { get; set; }
    }

    public class CommentDetailQuery
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public string MediaURL { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string AvatarUrl { get; set; }
        public List<CommentReactQuery> Reacts { get; set; } = new List<CommentReactQuery>();
    }
}
