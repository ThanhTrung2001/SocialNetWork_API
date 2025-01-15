namespace EnVietSocialNetWorkAPI.Model.Commands
{
    public class CreateChatGroupCommand
    {
        public string Name { get; set; }
        public string Group_Type { get; set; } //1-1 or group
        public string Theme { get; set; }
        public List<ModifyChatGroupUserCommand> Users { get; set; } = new List<ModifyChatGroupUserCommand>();
    }

    public class EditChatGroupCommand
    {
        public string Name { get; set; }
        public string Group_Type { get; set; } //1-1 or group
        public string Theme { get; set; }
    }

    public class AddUsersToChatGroupCommand
    {
        public List<ModifyChatGroupUserCommand> Users { get; set; } = new List<ModifyChatGroupUserCommand>();
    }

    public class ModifyChatGroupUserCommand
    {
        public Guid User_Id { get; set; }
        public string Role { get; set; }
    }

    public class ChangeNotificationCommand
    {
        public Guid User_Id { get; set; }
        public bool Is_Not_Notification { get; set; }
        public DateTime Delay_Until { get; set; }
    }

    public class DeleteChatGroupUsersCommand
    {
        public List<Guid> Users { get; set; } = new List<Guid>();
    }

    public class DeleteChatGroupUserCommand
    {
        public Guid User_Id { get; set; }
    }
}
