using System;
using System.Collections.Generic;

namespace EnVietSocialNetWorkAPI.Models;

public partial class User
{
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;

    public int Role { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    public virtual UserRole RoleNavigation { get; set; } = null!;

    public virtual ICollection<SharePost> SharePosts { get; set; } = new List<SharePost>();

    public virtual ICollection<UserDetail> UserDetails { get; set; } = new List<UserDetail>();

    public virtual ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();

    public virtual ICollection<UserReactComment> UserReactComments { get; set; } = new List<UserReactComment>();

    public virtual ICollection<UserReactMessage> UserReactMessages { get; set; } = new List<UserReactMessage>();

    public virtual ICollection<UserReactPost> UserReactPosts { get; set; } = new List<UserReactPost>();

    public virtual ICollection<ChatGroup> ChatGroups { get; set; } = new List<ChatGroup>();

    public virtual ICollection<SurveyItem> SurveyItems { get; set; } = new List<SurveyItem>();
}
