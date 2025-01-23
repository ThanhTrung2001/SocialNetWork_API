using System;
using System.Collections.Generic;

namespace EV.Model.Models;

public partial class Group
{
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public string Name { get; set; } = null!;

    public string? Avatar { get; set; }

    public string? Wallpaper { get; set; }

    public virtual ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();

    public virtual ICollection<UserRequestGroup> UserRequestGroups { get; set; } = new List<UserRequestGroup>();
}
