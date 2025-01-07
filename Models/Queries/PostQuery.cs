namespace EnVietSocialNetWorkAPI.Model.Queries
{
    public class PostQuery
    {
        public Guid PostId { get; set; }
        public string Content { get; set; }
        public int PostTypeId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool InGroup { get; set; }
        public Guid? DestinationId { get; set; }
        public int ReactCount { get; set; }

        public Guid UserId { get; set; }
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
        public Guid CommentId { get; set; }
        public Guid CommentUserId { get; set; }
        public string CommentFirstName { get; set; }
        public string CommentLastName { get; set; }
        public string CommentAvatar { get; set; }
        public string CommentContent { get; set; }
        public DateTime CommentCreatedAt { get; set; }
        public List<AttachmentQuery> Attachments { get; set; } = new List<AttachmentQuery>();
    }

    public class PostSurveyQuery
    {
        public Guid SurveyId { get; set; }
        public DateTime ExpiredAt { get; set; }
        public string Question { get; set; }
        public int Total { get; set; }
        public List<PostSurveyItemQuery> SurveyItems { get; set; } = new List<PostSurveyItemQuery>();
    }

    public class PostSurveyItemQuery
    {
        public Guid SurveyItemId { get; set; }
        public string SurveyItemName { get; set; }
        public int ItemTotal { get; set; }
        public List<PostVoteQuery> Votes { get; set; } = new List<PostVoteQuery>();
    }

    public class PostVoteQuery
    {
        public Guid UserVoteId { get; set; }
        public string VoteFirstName { get; set; }
        public string VoteLastName { get; set; }
        public string VoteAvatar { get; set; }
    }
}
