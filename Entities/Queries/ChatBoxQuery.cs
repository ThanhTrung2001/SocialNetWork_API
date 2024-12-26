namespace EnVietSocialNetWorkAPI.Entities.Queries
{
    public class ChatBoxQuery
    {
        public Guid ChatBoxId { get; set; }
        public string BoxType { get; set; }
    }

    //public class ChatBoxItemQuery
    //{
    //    public Guid UserId { get; set; }
    //    public string UserName { get; set; }
    //    public Guid ChatBoxId { get; set; }
    //    public string BoxType { get; set; }
    //}

    public class ChatBoxByUserIDQuery
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public List<ChatBoxQuery> BoxList { get; set; }

    }
}
