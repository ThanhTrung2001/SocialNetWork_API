namespace EnVietSocialNetWorkAPI.Model.Commands
{
    public class CreateCommentCommand
    {
        public Guid Post_Id { get; set; }
        public string Content { get; set; }
        public Guid User_Id { get; set; }
        public bool Is_Response { get; set; }
        public bool Is_SharePost { get; set; }
        public Guid? Comment_Id { get; set; }
        public List<CreateAttachmentCommand>? Attachments { get; set; } = new List<CreateAttachmentCommand>();

    }

    public class EditCommentCommand
    {
        public string Content { get; set; }
        public Guid User_Id { get; set; }
    }
}
