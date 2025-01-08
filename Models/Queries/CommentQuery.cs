namespace EnVietSocialNetWorkAPI.Model.Queries
{
    public class CommentQuery
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public bool Is_Response { get; set; }
        public bool Is_SharePost { get; set; }
        public int React_Count { get; set; }
        public DateTime Updated_At { get; set; }
        public Guid User_Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Avatar { get; set; }
        public List<AttachmentQuery>? Attachments { get; set; } = new List<AttachmentQuery>();

    }

    public class CommentDetailQuery
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public bool Is_Response { get; set; }
        public bool Is_SharePost { get; set; }
        public int React_Count { get; set; }
        public DateTime Updated_At { get; set; }
        public Guid User_Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Avatar { get; set; }
        public List<CommentReactQuery>? Reacts { get; set; } = new List<CommentReactQuery>();
        public List<AttachmentQuery>? Attachments { get; set; } = new List<AttachmentQuery>();

    }
}
