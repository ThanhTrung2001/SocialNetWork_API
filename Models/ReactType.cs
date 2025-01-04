using System;
using System.Collections.Generic;

namespace EnVietSocialNetWorkAPI.Models;

public partial class ReactType
{
    public int Id { get; set; }

    public string TypeName { get; set; } = null!;

    public virtual ICollection<UserReactComment> UserReactComments { get; set; } = new List<UserReactComment>();

    public virtual ICollection<UserReactMessage> UserReactMessages { get; set; } = new List<UserReactMessage>();

    public virtual ICollection<UserReactPost> UserReactPosts { get; set; } = new List<UserReactPost>();
}
