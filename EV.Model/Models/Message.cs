using System;
using System.Collections.Generic;

namespace EV.Model.Models;

public partial class Message
{
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public Guid SenderId { get; set; }

    public Guid ChatgroupId { get; set; }

    public string Content { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string Type { get; set; } = null!;

    public int ReactCount { get; set; }

    public bool IsResponse { get; set; }

    public bool IsPinned { get; set; }

    public virtual ChatGroup Chatgroup { get; set; } = null!;

    public virtual User Sender { get; set; } = null!;

    public virtual ICollection<UserReactMessage> UserReactMessages { get; set; } = new List<UserReactMessage>();

    public virtual ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual ICollection<Message> Responses { get; set; } = new List<Message>();
}
