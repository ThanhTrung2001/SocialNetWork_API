using System;
using System.Collections.Generic;

namespace EV.Model.Models;

public partial class UserChatgroup
{
    public Guid UserId { get; set; }

    public Guid ChatgroupId { get; set; }

    public bool IsNotNotification { get; set; }

    public DateTime DelayUntil { get; set; }

    public string Role { get; set; } = null!;

    public virtual ChatGroup Chatgroup { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
