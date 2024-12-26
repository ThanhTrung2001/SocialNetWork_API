namespace EnVietSocialNetWorkAPI.Entities.Models.SocialNetwork
{
    public class Post : BaseClass
    {
        public string? DestinationId { get; set; }
        public bool isNotification { get; set; }
        public string PostType { get; set; }
        public string PostDetination { get; set; }
        public string? Content { get; set; }

        public User Owner { get; set; }
        public ICollection<Comment>? Comments { get; set; }
        public ICollection<React>? Reacts { get; set; }
        public ICollection<Survey>? Surveys { get; set; }
        public ICollection<MediaItem>? Medias { get; set; }

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
