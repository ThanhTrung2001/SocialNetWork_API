namespace EnVietSocialNetWorkAPI.Models.Queries
{
    public class GroupQuery
    {
        public Guid Id { get; set; }
        public string GroupName { get; set; }
        public string Avatar { get; set; }
        public string Wallpaper { get; set; }
        public List<UserGroupQuery> Users { get; set; } = new List<UserGroupQuery>();
    }

    public class UserGroupQuery
    {
        public Guid UserId { get; set; }
        public int Role { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserAvatar { get; set; }
        public DateTime JoinedAt { get; set; }
    }
}
