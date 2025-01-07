namespace EnVietSocialNetWorkAPI.Models;

public partial class UserReactMessage
{
    public Guid UserId { get; set; }

    public Guid MessageId { get; set; }

    public string ReactType { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Message Message { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
