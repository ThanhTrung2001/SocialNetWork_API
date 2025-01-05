namespace EnVietSocialNetWorkAPI.Models.Queries
{
    public class SharePostQuery
    {
        public Guid Id { get; set; }
        public string ShareContent { get; set; }
        public DateTime ShareCreatedAt { get; set; }
        public Guid ShareUserId { get; set; }
        public string ShareUserName { get; set; }
        public string ShareUserAvatar { get; set; }

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
        public PostSurveyQuery Survey { get; set; }
    }
}
