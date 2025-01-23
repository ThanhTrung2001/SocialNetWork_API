namespace EnVietSocialNetWorkAPI.Model.Queries
{
    public class GroupQuery
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public string Wallpaper { get; set; }
        public List<UserGroupQuery> Users { get; set; } = new List<UserGroupQuery>();
    }

    public class UserGroupQuery
    {
        public Guid User_Id { get; set; }
        public string Role { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string User_Avatar { get; set; }
        public DateTime Joined_At { get; set; }
    }
}
