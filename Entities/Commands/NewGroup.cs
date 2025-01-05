﻿namespace EnVietSocialNetWorkAPI.Entities.Commands
{
    public class GroupCommand
    {
        public string? GroupName { get; set; }
        public string? WallapperURL { get; set; }
        public List<Guid>? Users { get; set; } = new List<Guid>();
    }
}
