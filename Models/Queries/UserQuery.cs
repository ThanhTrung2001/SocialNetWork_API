namespace EnVietSocialNetWorkAPI.Models.Queries
{
    public class UserQuery
    {
        public Guid Id { get; set; }
        //public string UserName { get; set; }
        //public string Email { get; set; }
        public int Role { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Avatar { get; set; }
    }

    public class UserQueryDetail
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public int RoleId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Avatar { get; set; }
        public string Wallpaper { get; set; }
        public DateTime DOB { get; set; }
        public string Bio { get; set; }
    }
}
