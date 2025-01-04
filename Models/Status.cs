using System;
using System.Collections.Generic;

namespace EnVietSocialNetWorkAPI.Models;

public partial class Status
{
    public int Id { get; set; }

    public string StatusName { get; set; } = null!;

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}
