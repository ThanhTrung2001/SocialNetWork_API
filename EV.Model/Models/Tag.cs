using System;
using System.Collections.Generic;

namespace EV.Model.Models;

public partial class Tag
{
    public int Id { get; set; }

    public string TagName { get; set; } = null!;

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
}
