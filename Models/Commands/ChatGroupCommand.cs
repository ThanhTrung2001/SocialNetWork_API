namespace EnVietSocialNetWorkAPI.Model.Commands
{
    public class CreateChatGroupCommand
    {
        public string ChatName { get; set; }
        public string GroupType { get; set; } //1-1 or group
        public int Theme { get; set; }
        public List<Guid> Users { get; set; } = new List<Guid>();
    }

    public class EditChatGroupCommand
    {
        public string ChatName { get; set; }
        public string GroupType { get; set; } //1-1 or group
        public int Theme { get; set; }
    }

    public class AddUsersToChatGroupCommand
    {
        public List<Guid> Users { get; set; } = new List<Guid>();
    }
}
