namespace EnVietSocialNetWorkAPI.Models.Queries
{
    public class PageQuery
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public string Wallpaper { get; set; }
        public DateTime Created_At { get; set; }
    }
}
