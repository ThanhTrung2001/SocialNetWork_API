namespace EV.Model.Handlers.Queries
{
    public class MessageQuery
    {
        public Guid Id { get; set; }
        public DateTime Created_At { get; set; }
        public Guid Sender_Id { get; set; }
        public Guid ChatGroup_Id { get; set; }
        public string Content { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Avatar { get; set; }
        public bool Is_Pinned { get; set; }
        public bool Is_Response { get; set; }
        public int React_Count { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public List<AttachmentQuery>? Attachments { get; set; } = new List<AttachmentQuery>();
    }

    public class MessageDetailQuery
    {
        public Guid Id { get; set; }
        public DateTime Created_At { get; set; }
        public Guid Sender_Id { get; set; }
        public Guid ChatGroup_Id { get; set; }
        public string Content { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Avatar { get; set; }
        public bool Is_Pinned { get; set; }
        public bool Is_Response { get; set; }
        public int React_Count { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public List<AttachmentQuery>? Attachments { get; set; } = new List<AttachmentQuery>();
        public List<MessageReactQuery>? Reacts { get; set; } = new List<MessageReactQuery>();
    }
}
