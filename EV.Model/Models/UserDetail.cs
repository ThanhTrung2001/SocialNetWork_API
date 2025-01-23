using System;
using System.Collections.Generic;

namespace EV.Model.Models;

public partial class UserDetail
{
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public string Firstname { get; set; } = null!;

    public string Lastname { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string City { get; set; } = null!;

    public string Country { get; set; } = null!;

    public string? Avatar { get; set; }

    public string? Wallpaper { get; set; }

    public DateTime Dob { get; set; }

    public string? Bio { get; set; }

    public Guid UserId { get; set; }

    public virtual User User { get; set; } = null!;
}
