namespace EnVietSocialNetWorkAPI.Entities.Queries
{
    public class GroupQuery
    {
        public Guid Id { get; set; }
        public string GroupName { get; set; }
        public List<UserGroupQuery> Users { get; set; } = new List<UserGroupQuery>();
    }

    public class UserGroupQuery : UserQuery
    {
        public int RoleId { get; set; }
    }
}
