namespace EnVietSocialNetWorkAPI.Models;

public partial class Notification
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int NotiType { get; set; }

    public Guid? DestinationId { get; set; }

    public string? OrganizationName { get; set; }

    public DateTime Startedat { get; set; }

    public DateTime? Endedat { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
}
