namespace EnVietSocialNetWorkAPI.Models.Commands
{
    public class CreatePageCommand
    {
        public string Name { get; set; }
        public string Avatar { get; set; }
        public string Wallpaper { get; set; }
        public Guid User_Id { get; set; }
    }

    public class EditPageCommand
    {
        public string Name { get; set; }
        public string Avatar { get; set; }
        public string Wallpaper { get; set; }
    }

    public class ModifyPageUserCommand
    {
        public Guid User_Id { get; set; }
        public string Role { get; set; }
    }

    public class ModifyFollowPageCommand
    {
        public Guid User_Id { get; set; }
    }
}
