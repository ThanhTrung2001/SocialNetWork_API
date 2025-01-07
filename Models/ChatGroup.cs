namespace EnVietSocialNetWorkAPI.Models;

public partial class ChatGroup
{
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public string Name { get; set; } = null!;

    public string Theme { get; set; } = null!;

    public string GroupType { get; set; } = null!;

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual ICollection<UserChatgroup> UserChatgroups { get; set; } = new List<UserChatgroup>();
}
