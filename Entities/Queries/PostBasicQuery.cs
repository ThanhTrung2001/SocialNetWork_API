namespace EnVietSocialNetWorkAPI.Entities.Queries
{
    public class PostBasicQuery
    {
        public Guid PostId { get; set; }
        public string PostContent { get; set; }
        public string PostType { get; set; }

        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string AvatarUrl { get; set; }

        public List<string> MediaUrls { get; set; } = new List<string>();

    }
}
