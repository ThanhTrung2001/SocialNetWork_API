namespace EnVietSocialNetWorkAPI.Models;

public partial class Attachment
{
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public string Media { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
}
