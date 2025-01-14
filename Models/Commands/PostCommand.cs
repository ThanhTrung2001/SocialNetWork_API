namespace EnVietSocialNetWorkAPI.Model.Commands
{
    public class CreatePostCommand
    {
        public bool In_Group { get; set; }
        public Guid? Destination_Id { get; set; }
        public string Post_Type { get; set; }
        public string Content { get; set; }
        public List<CreateAttachmentCommand>? Attachments { get; set; } = new List<CreateAttachmentCommand>();
        public CreateSurveyCommand? Survey { get; set; }

    }

    public class CreatePostRequest
    {
        public Guid User_Id { get; set; }
        public CreatePostCommand NewPost { get; set; }
    }

    public class EditPostRequest
    {
        public Guid User_Id { get; set; }
        public string Post_Type { get; set; }
        public string Content { get; set; }
    }
}
