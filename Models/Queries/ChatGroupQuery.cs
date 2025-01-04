namespace EnVietSocialNetWorkAPI.Models.Queries
{
    public class ChatGroupQuery
    {
        public Guid Id { get; set; }
        public string BoxName { get; set; }
        public string BoxType { get; set; }
        public string Theme { get; set; }
        public List<Guid> Users { get; set; } = new List<Guid>();
    }

    //public class ChatBoxItemQuery
    //{
    //    public Guid UserId { get; set; }
    //    public string UserName { get; set; }
    //    public Guid ChatBoxId { get; set; }
    //    public string BoxType { get; set; }
    //}

    public class ChatGroupByUserIDQuery
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public List<ChatGroupQuery> BoxList { get; set; }

    }
}
