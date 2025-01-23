using System;
using System.Collections.Generic;

namespace EV.Model.Models;

public partial class UserPage
{
    public Guid UserId { get; set; }

    public Guid PageId { get; set; }

    public string Role { get; set; } = null!;

    public bool IsFollow { get; set; }

    public DateTime JoinedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Page Page { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
