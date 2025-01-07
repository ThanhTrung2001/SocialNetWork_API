namespace EnVietSocialNetWorkAPI.Models;

public partial class UserGroup
{
    public Guid UserId { get; set; }

    public Guid GroupId { get; set; }

    public string Role { get; set; } = null!;

    public bool IsFollow { get; set; }

    public DateTime JoinedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Group Group { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
