﻿namespace EV.Model.Handlers.Commands
{
    public class CreateGroupCommand
    {
        public string Name { get; set; }
        public string? Avatar { get; set; }
        public string? Wallapper { get; set; }
        public List<ModifyGroupUserCommand>? Users { get; set; } = new List<ModifyGroupUserCommand>();
    }

    public class EditGroupCommand
    {
        public string Name { get; set; }
        public string? Avatar { get; set; }
        public string? Wallapper { get; set; }
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

    public class DeleteGroupUsersCommand
    {
        public List<DeleteGroupUserCommand> Users { get; set; } = new List<DeleteGroupUserCommand>();
    }

    public class DeleteGroupUserCommand
    {
        public Guid User_Id { get; set; }
    }
}
