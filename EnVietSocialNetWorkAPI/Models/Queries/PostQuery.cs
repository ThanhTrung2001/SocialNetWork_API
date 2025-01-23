namespace EnVietSocialNetWorkAPI.Model.Queries
{
    public class PostQuery
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public string Post_Type { get; set; }
        public DateTime Created_At { get; set; }
        //public DateTime Updated_At { get; set; }
        public bool In_Group { get; set; }
        public Guid? Destination_Id { get; set; }
        public int React_Count { get; set; }

        public Guid User_Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Avatar { get; set; }

        public List<AttachmentQuery> Attachments { get; set; } = new List<AttachmentQuery>();
        public List<PostCommentQuery> Comments { get; set; } = new List<PostCommentQuery>();
        public List<PostReactQuery> Reacts { get; set; } = new List<PostReactQuery>();
        //ShareQuery
        public PostSurveyQuery? Survey { get; set; }
    }

    public class PostCommentQuery
    {
        public Guid Comment_Id { get; set; }
        public Guid Comment_UserId { get; set; }
        public string Comment_FirstName { get; set; }
        public string Comment_LastName { get; set; }
        public string Comment_Avatar { get; set; }
        public string Comment_Content { get; set; }
        public DateTime Comment_CreatedAt { get; set; }
        public List<AttachmentQuery>? Attachments { get; set; } = new List<AttachmentQuery>();
    }

    public class PostSurveyQuery
    {
        public Guid Survey_Id { get; set; }
        public DateTime Expired_At { get; set; }
        public string Question { get; set; }
        public string Survey_Type { get; set; }
        public int Total_Vote { get; set; }
        public List<PostSurveyItemQuery> SurveyItems { get; set; } = new List<PostSurveyItemQuery>();
    }

    public class PostSurveyItemQuery
    {
        public Guid SurveyItem_Id { get; set; }
        public string SurveyItem_Name { get; set; }
        public int Item_Total { get; set; }
        public List<PostVoteQuery> Votes { get; set; } = new List<PostVoteQuery>();
    }

    public class PostVoteQuery
    {
        public Guid Vote_UserId { get; set; }
        public string Vote_FirstName { get; set; }
        public string Vote_LastName { get; set; }
        public string Vote_Avatar { get; set; }
    }
}
