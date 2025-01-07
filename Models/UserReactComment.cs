namespace EnVietSocialNetWorkAPI.Models;

public partial class UserReactComment
{
    public Guid UserId { get; set; }

    public Guid Commentid { get; set; }

    public string ReactType { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Comment Comment { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
