namespace EnVietSocialNetWorkAPI.Model.Queries
{
    public class SharePostQuery
    {
        public Guid Id { get; set; }
        public string ShareContent { get; set; }
        public DateTime ShareCreatedAt { get; set; }
        public Guid SharedByUserId { get; set; }
        public bool ShareInGroup { get; set; }
        public string ShareFirstName { get; set; }
        public string ShareLastName { get; set; }
        public string ShareAvatar { get; set; }

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
        public PostSurveyQuery? Survey { get; set; }
    }

    public class ShareUserQuery
    {
        public Guid Id { get; set; }
        //public string ShareContent { get; set; }
        public Guid ShareUserId { get; set; }
        public string ShareFirstName { get; set; }
        public string ShareLastName { get; set; }
        public string ShareAvatar { get; set; }
    }


}
