namespace EnVietSocialNetWorkAPI.Models.Queries
{
    public class MessageQuery
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid SenderId { get; set; }
        public Guid ChatGroupId { get; set; }
        public string Content { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Avatar { get; set; }
        public bool IsPinned { get; set; }
        public bool IsResponse { get; set; }
        public int ReactCount { get; set; }
        public int MessageTypeId { get; set; }
        public int StatusId { get; set; }
    }

    public class MessageDetailQuery
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid SenderId { get; set; }
        public Guid ChatGroupId { get; set; }
        public string Content { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Avatar { get; set; }
        public bool IsPinned { get; set; }
        public bool IsResponse { get; set; }
        public int ReactCount { get; set; }
        public int MessageType { get; set; }
        public int StatusId { get; set; }
        public List<MessageReactQuery> Reacts { get; set; } = new List<MessageReactQuery>();
    }
}
