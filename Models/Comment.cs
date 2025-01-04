namespace EnVietSocialNetWorkAPI.Models;

public partial class Comment
{
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public string Content { get; set; } = null!;

    public bool IsResponse { get; set; }

    public int ReactCount { get; set; }

    public Guid UserId { get; set; }

    public Guid PostId { get; set; }

    public virtual Post Post { get; set; } = null!;

    public virtual User User { get; set; } = null!;

    public virtual ICollection<UserReactComment> UserReactComments { get; set; } = new List<UserReactComment>();

    public virtual ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Comment> Responses { get; set; } = new List<Comment>();
}
