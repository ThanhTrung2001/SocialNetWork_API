namespace EnVietSocialNetWorkAPI.Model.Commands
{
    public class CreateUserCommand
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone_Number { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
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
        public string Role { get; set; }
    }

    public class ChangeUserPasswordCommand
    {
        public string Password { get; set; }
    }

    public class EditUserDetailCommand
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone_Number { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Avatar { get; set; }
        public string Wallpaper { get; set; }
        public DateTime DOB { get; set; }
        public string Bio { get; set; }
    }
}
