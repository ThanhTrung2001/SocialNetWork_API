namespace EV.Model.Models;

public partial class UserRequestGroup
{
    public Guid UserId { get; set; }

    public Guid GroupId { get; set; }

    public string Status { get; set; } = null!;

    public bool IsAdminRequest { get; set; } = false;

    public Guid? AdminId { get; set; } = null;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Group Group { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
