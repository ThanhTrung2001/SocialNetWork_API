using System;
using System.Collections.Generic;

namespace EnVietSocialNetWorkAPI.Models;

public partial class MessageType
{
    public int Id { get; set; }

    public string MessageType1 { get; set; } = null!;

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}
