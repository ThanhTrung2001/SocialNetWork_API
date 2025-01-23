using System;
using System.Collections.Generic;

namespace EnVietSocialNetWorkAPI.Models.Entities;

public partial class UserOrganization
{
    public Guid UserId { get; set; }

    public Guid NodeId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public string OrganizationRole { get; set; } = null!;

    public virtual Organization Node { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
