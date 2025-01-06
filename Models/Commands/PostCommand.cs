namespace EnVietSocialNetWorkAPI.Models.Commands
{
    public class CreatePostCommand
    {
        public bool InGroup { get; set; }
        public Guid? DestinationId { get; set; }
        public int PostTypeId { get; set; }
        public string Content { get; set; }
        public List<CreateAttachmentCommand>? Attachments { get; set; } = new List<CreateAttachmentCommand>();
        public CreateSurveyCommand? Survey { get; set; }

    }

    public class CreatePostRequest
    {
        public Guid UserId { get; set; }
        public CreatePostCommand NewPost { get; set; }
    }
}
