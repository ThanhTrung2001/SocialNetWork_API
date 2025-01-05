namespace EnVietSocialNetWorkAPI.Models;

public partial class ChatGroup
{
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public string ChatName { get; set; } = null!;

    public int ThemeId { get; set; }

    public string GroupType { get; set; } = null!;

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual Theme ThemeNavigation { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
