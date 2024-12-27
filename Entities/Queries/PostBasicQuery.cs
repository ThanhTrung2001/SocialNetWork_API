namespace EnVietSocialNetWorkAPI.Entities.Queries
{
    public class PostBasicQuery
    {
        public Guid PostId { get; set; }
        public string PostContent { get; set; }
        public string PostType { get; set; }
        public DateTime CreatedAt { get; set; }
        public string PostDestination { get; set; }

        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string AvatarUrl { get; set; }

        public List<string> MediaUrls { get; set; } = new List<string>();
        public List<PostCommentQuery> Comments { get; set; } = new List<PostCommentQuery>();

    }

    public class PostCommentQuery
    {
        public Guid CommentId { get; set; }
        public Guid CommentUserId { get; set; }
        public string CommentUserName { get; set; }
        public string CommentUserAvatarUrl { get; set; }
        public string CommentContent { get; set; }
        public string? CommentMediaUrl { get; set; }
        public DateTime CommentCreatedAt { get; set; }
    }
}
