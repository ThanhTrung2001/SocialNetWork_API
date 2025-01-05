namespace EnVietSocialNetWorkAPI.Models.Commands
{
    public class CreateMessageCommand
    {
        public string? Content { get; set; }
        public Guid SenderId { get; set; }
        public bool IsResponse { get; set; }
        public int MessageTypeId { get; set; }
        //public bool IsPinned { get; set; }
        //public int Status { get; set; }
        //public Guid ChatGroupId { get; set; }
        //public ICollection<MessageReact>? Reacts { get; set; }
    }
}
