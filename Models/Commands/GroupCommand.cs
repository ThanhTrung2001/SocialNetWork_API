namespace EnVietSocialNetWorkAPI.Model.Commands
{
    public class CreateGroupCommand
    {
        public string Name { get; set; }
        public string? Avatar { get; set; }
        public string? Wallapper { get; set; }
        public List<ModifyGroupUserCommand>? Users { get; set; } = new List<ModifyGroupUserCommand>();
    }

    public class ModifyGroupUsersCommand
    {
        public List<ModifyGroupUserCommand> Users { get; set; } = new List<ModifyGroupUserCommand>();
    }

    public class ModifyGroupUserCommand
    {
        public Guid User_Id { get; set; }
        public string Role { get; set; }
        public bool? Is_Follow { get; set; } = true;
    }

    public class DeleteGroupUserCommand
    {
        public Guid User_Id { get; set; }
    }
}
