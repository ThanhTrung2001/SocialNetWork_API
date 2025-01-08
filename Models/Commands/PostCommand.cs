namespace EnVietSocialNetWorkAPI.Model.Commands
{
    public class CreatePostCommand
    {
        public bool In_Group { get; set; }
        public Guid? Destination_Id { get; set; }
        public int Post_Type_Id { get; set; }
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
