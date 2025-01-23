namespace EnVietSocialNetWorkAPI.Model.Queries
{
    public class SharePostQuery
    {
        public Guid Id { get; set; }
        public string Share_Content { get; set; }
        public DateTime Share_CreatedAt { get; set; }
        public Guid Shared_By_User_Id { get; set; }
        public bool Share_InGroup { get; set; }
        public string Share_FirstName { get; set; }
        public string Share_LastName { get; set; }
        public string Share_Avatar { get; set; }
        public int React_Count { get; set; }

        public Guid Post_Id { get; set; }
        public string Content { get; set; }
        public string Post_Type { get; set; }
        public DateTime Created_At { get; set; }
        public DateTime Updated_At { get; set; }
        public bool In_Group { get; set; }
        public Guid? Destination_Id { get; set; }

        public Guid User_Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Avatar { get; set; }
        public List<AttachmentQuery> Attachments { get; set; } = new List<AttachmentQuery>();
        public PostSurveyQuery? Survey { get; set; }
        public List<PostCommentQuery> Comments { get; set; } = new List<PostCommentQuery>();
        public List<PostReactQuery> Reacts { get; set; } = new List<PostReactQuery>();
    }

    public class ShareUserQuery
    {
        public Guid Id { get; set; }
        //public string ShareContent { get; set; }
        public Guid Share_UserId { get; set; }
        public string Share_FirstName { get; set; }
        public string Share_LastName { get; set; }
        public string Share_Avatar { get; set; }
    }


}
