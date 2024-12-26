namespace EnVietSocialNetWorkAPI.Entities.Models.SocialNetwork
{
    public class MediaItem:BaseClass
    {
        public FileType FileType { get; set; }
        public string URL { get; set; }
        public Post post { get; set; }
    }

    public enum FileType
    {
        Image=1,
        Video=2,
        File = 3
    }
}
