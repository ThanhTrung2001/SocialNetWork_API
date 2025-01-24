namespace EV.Model.Handlers.Queries
{
    public class PageQuery
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public string Wallpaper { get; set; }
        public DateTime Created_At { get; set; }
    }

    public class PageQueryDetail
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public string Wallpaper { get; set; }
        public DateTime Created_At { get; set; }
        public List<UserPageQuery> Users { get; set; } = new List<UserPageQuery>();
    }

    public class UserPageQuery
    {
        public Guid User_Id { get; set; }
        public string Role { get; set; }
        public bool is_Follow { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string User_Avatar { get; set; }
        public DateTime Joined_At { get; set; }
    }
}
