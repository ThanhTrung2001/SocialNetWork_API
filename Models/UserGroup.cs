using System;
using System.Collections.Generic;

namespace EnVietSocialNetWorkAPI.Models;

public partial class UserGroup
{
    public Guid UserId { get; set; }

    public Guid GroupId { get; set; }

    public int Role { get; set; }

    public DateTime JoineddAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Group Group { get; set; } = null!;

    public virtual UserRole RoleNavigation { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
