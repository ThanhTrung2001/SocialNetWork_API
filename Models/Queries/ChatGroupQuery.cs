namespace EnVietSocialNetWorkAPI.Model.Queries
{
    public class ChatGroupQuery
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Group_Type { get; set; } //1-1 or group
        public string Theme { get; set; }
        public List<UserChatGroupQuery>? Users { get; set; } = new List<UserChatGroupQuery>();
    }

    public class ChatGroupByUserIDQuery
    {
        public Guid User_Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<ChatGroupQuery> ChatGroups { get; set; }
    }

    public class UserChatGroupQuery
    {
        public Guid User_Id { get; set; }
        public string Role { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Avatar { get; set; }
    }

}
