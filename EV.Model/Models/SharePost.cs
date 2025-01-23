using System;
using System.Collections.Generic;

namespace EV.Model.Models;

public partial class SharePost
{
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public Guid SharedPostId { get; set; }

    public Guid SharedByUserId { get; set; }

    public bool InGroup { get; set; }

    public Guid? DestinationId { get; set; }

    public string? Content { get; set; }

    public int ReactCount { get; set; }

    public virtual User SharedByUser { get; set; } = null!;

    public virtual Post SharedPost { get; set; } = null!;
}
