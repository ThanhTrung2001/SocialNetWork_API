namespace EnVietSocialNetWorkAPI.Models.Commands
{
    public class CreateGroupCommand
    {
        public string GroupName { get; set; }
        public string? Avatar { get; set; }
        public string? Wallapper { get; set; }
        public List<Guid>? Users { get; set; } = new List<Guid>();
    }

    public class AddUsersToGroupCommand
    {
        public List<Guid> Users { get; set; } = new List<Guid>();
    }

}
