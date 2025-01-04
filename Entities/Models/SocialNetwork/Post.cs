namespace EnVietSocialNetWorkAPI.Entities.Models.SocialNetwork
{
    public class Post : BaseClass
    {
        public Guid UserId { get; set; }
        public string? Content { get; set; }
        public PostType PostType { get; set; }
        public bool InGroup { get; set; }
        public Guid? DestinationId { get; set; }

    }

    public enum PostType
    {
        Normal = 1,
        Survey = 2,
        Notification = 3
    }

    public enum PostDestination
    {
        PersonalWall = 1,
        Group = 2,
        Company = 3
    }
}
