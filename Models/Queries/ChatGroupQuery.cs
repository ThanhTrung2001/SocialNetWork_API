namespace EnVietSocialNetWorkAPI.Models.Queries
{
    public class ChatGroupQuery
    {
        public Guid Id { get; set; }
        public string ChatName { get; set; }
        public string GroupType { get; set; } //1-1 or group
        public int Theme { get; set; }
        public List<UserChatGroupQuery> Users { get; set; } = new List<UserChatGroupQuery>();
    }

    public class ChatGroupByUserIDQuery
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public List<ChatGroupQuery> BoxList { get; set; }
    }

    public class UserChatGroupQuery
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Avatar { get; set; }
    }
}
