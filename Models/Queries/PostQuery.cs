namespace EnVietSocialNetWorkAPI.Models.Queries
{
    public class PostQuery
    {
        public Guid PostId { get; set; }
        public string Content { get; set; }
        public int PostTypeId { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid? DestinationId { get; set; }

        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AvatarUrl { get; set; }

        public List<AttachmentQuery> Attachments { get; set; } = new List<AttachmentQuery>();
        public List<PostCommentQuery> Comments { get; set; } = new List<PostCommentQuery>();
        public List<PostReactQuery> Reacts { get; set; } = new List<PostReactQuery>();
        //ShareQuery
        public PostSurveyQuery? Survey { get; set; }
    }

    public class AttachmentQuery
    {

    }

    public class PostCommentQuery
    {
        public Guid CommentId { get; set; }
        public Guid CommentUserId { get; set; }
        public string CommentFirstName { get; set; }
        public string CommentLastName { get; set; }
        public string CommentUserAvatarUrl { get; set; }
        public string CommentContent { get; set; }
        public DateTime CommentCreatedAt { get; set; }
        public List<AttachmentQuery> Attachments { get; set; } = new List<AttachmentQuery>();
    }

    public class PostSurveyQuery
    {
        public Guid SurveyId { get; set; }
        public DateTime ExpiredIn { get; set; }
        public string SurveyQuestion { get; set; }
        public List<SurveyItemQuery> SurveyItems { get; set; } = new List<SurveyItemQuery>();
    }

    public class SurveyItemQuery
    {
        public Guid SurveyItemId { get; set; }
        public string SurveyItemContent { get; set; }
        public int SurveyItemVotes { get; set; }
        public List<SurveyItemVote> SurveyVotes { get; set; } = new List<SurveyItemVote>();
    }

    public class SurveyItemVote
    {
        public Guid VoteId { get; set; }
        public Guid VoteUserId { get; set; }
        public string VoteUserName { get; set; }
        public string VoteUserAvatar { get; set; }
    }

    public class ShareUserQuery
    {
        public Guid Id { get; set; }
        public string ShareContent { get; set; }
        public Guid ShareUserId { get; set; }
        public string ShareUserName { get; set; }
        public string ShareUserAvatar { get; set; }
    }

    public class SharePostQuery
    {
        public Guid Id { get; set; }
        public string ShareContent { get; set; }
        public DateTime ShareCreatedAt { get; set; }
        public Guid ShareUserId { get; set; }
        public string ShareUserName { get; set; }
        public string ShareUserAvatar { get; set; }

        public Guid PostId { get; set; }
        public string PostContent { get; set; }
        public string PostType { get; set; }
        public DateTime CreatedAt { get; set; }
        public string PostDestination { get; set; }

        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string AvatarUrl { get; set; }

        public List<string> MediaUrls { get; set; } = new List<string>();
        public PostSurveyQuery Survey { get; set; }
    }
}
