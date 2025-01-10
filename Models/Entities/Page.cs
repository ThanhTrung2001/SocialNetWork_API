using System;
using System.Collections.Generic;

namespace EnVietSocialNetWorkAPI.Models.Entities;

public partial class Page
{
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public string Name { get; set; } = null!;

    public string? Avatar { get; set; }

    public string? Wallpaper { get; set; }

    public virtual ICollection<UserPage> UserPages { get; set; } = new List<UserPage>();
}
