namespace EnVietSocialNetWorkAPI.Entities.Models.SocialNetwork
{
    public class Group : BaseClass
    {
        public string GroupName { get; set; }
        public string WallpaperURL { get; set; }

        public ICollection<User> Users { get; set; }
        public ICollection<Post> Posts { get; set; }
    }
}
