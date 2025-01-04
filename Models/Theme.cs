using System;
using System.Collections.Generic;

namespace EnVietSocialNetWorkAPI.Models;

public partial class Theme
{
    public int Id { get; set; }

    public string Theme1 { get; set; } = null!;

    public virtual ICollection<ChatGroup> ChatGroups { get; set; } = new List<ChatGroup>();
}
