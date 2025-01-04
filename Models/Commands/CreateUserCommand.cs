namespace EnVietSocialNetWorkAPI.Models.Commands
{
    public class CreateUserCommand
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public int Role { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Avatar { get; set; }
        public string Wallpaper { get; set; }
        public DateTime DOB { get; set; }
        public string Bio { get; set; }
    }

    public class EditUserCommand
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public int Role { get; set; }
    }

    public class EditUserDetailCommand
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Avatar { get; set; }
        public string Wallpaper { get; set; }
        public DateTime DOB { get; set; }
        public string Bio { get; set; }
    }
}
