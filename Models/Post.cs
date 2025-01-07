namespace EnVietSocialNetWorkAPI.Models;

public partial class Post
{
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public Guid? DestinationId { get; set; }

    public bool InGroup { get; set; }

    public string? Content { get; set; }

    public int ReactCount { get; set; }

    public Guid UserId { get; set; }

    public int PostTypeId { get; set; }

    public virtual PostType PostType { get; set; } = null!;

    public virtual ICollection<SharePost> SharePosts { get; set; } = new List<SharePost>();

    public virtual ICollection<Survey> Surveys { get; set; } = new List<Survey>();

    public virtual ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();

    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
}
