namespace EnVietSocialNetWorkAPI.Models;

public partial class UserReactPost
{
    public Guid UserId { get; set; }

    public Guid PostId { get; set; }

    public bool IsSharepost { get; set; }

    public string ReactType { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual User User { get; set; } = null!;
}
