using System;
using System.Collections.Generic;

namespace EnVietSocialNetWorkAPI.Models;

public partial class UserReactComment
{
    public Guid UserId { get; set; }

    public Guid CommentId { get; set; }

    public int ReactTypeId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Comment Comment { get; set; } = null!;

    public virtual ReactType ReactType { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
