using System;
using System.Collections.Generic;

namespace EV.Model.Models;

public partial class PostType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
}
