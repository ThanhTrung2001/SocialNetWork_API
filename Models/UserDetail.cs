using System;
using System.Collections.Generic;

namespace EnVietSocialNetWorkAPI.Models;

public partial class UserDetail
{
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Avatar { get; set; }

    public string? Wallpaper { get; set; }

    public DateTime Dob { get; set; }

    public string? Bio { get; set; }

    public Guid UserId { get; set; }

    public virtual User User { get; set; } = null!;
}
