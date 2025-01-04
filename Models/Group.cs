using System;
using System.Collections.Generic;

namespace EnVietSocialNetWorkAPI.Models;

public partial class Group
{
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public string GroupName { get; set; } = null!;

    public string? Avatar { get; set; }

    public string? Wallpaper { get; set; }

    public virtual ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();
}
