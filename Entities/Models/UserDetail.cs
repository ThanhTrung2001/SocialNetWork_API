namespace EnVietSocialNetWorkAPI.Entities.Models
{
    public class UserDetail : BaseClass
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Avatar { get; set; }
        public string Wallpaper { get; set; }
        public DateTime DOB { get; set; }
        public string Bio { get; set; }
    }
}
