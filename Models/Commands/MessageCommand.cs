namespace EnVietSocialNetWorkAPI.Model.Commands
{
    public class CreateMessageCommand
    {
        public Guid ChatGroup_Id { get; set; }
        public string? Content { get; set; }
        public Guid Sender_Id { get; set; }
        public bool Is_Response { get; set; }
        public string Type { get; set; }
        public List<CreateAttachmentCommand>? Attachments { get; set; } = new List<CreateAttachmentCommand>();

        //public bool IsPinned { get; set; }
        //public int Status { get; set; }
        //public Guid ChatGroupId { get; set; }
        //public ICollection<MessageReact>? Reacts { get; set; }

    }

    public class PinMessageCommand
    {
        public Guid Sender_Id { get; set; }
    }
}
