using System;
using System.Collections.Generic;

namespace EV.Model.Models;

public partial class User
{
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Role { get; set; } = null!;

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    public virtual ICollection<SharePost> SharePosts { get; set; } = new List<SharePost>();

    public virtual ICollection<UserChatgroup> UserChatgroups { get; set; } = new List<UserChatgroup>();

    public virtual ICollection<UserDetail> UserDetails { get; set; } = new List<UserDetail>();

    public virtual ICollection<UserGroup> UserGroups { get; set; } = new List<UserGroup>();

    public virtual ICollection<UserOrganization> UserOrganizations { get; set; } = new List<UserOrganization>();

    public virtual ICollection<UserPage> UserPages { get; set; } = new List<UserPage>();

    public virtual ICollection<UserReactComment> UserReactComments { get; set; } = new List<UserReactComment>();

    public virtual ICollection<UserReactMessage> UserReactMessages { get; set; } = new List<UserReactMessage>();

    public virtual ICollection<UserReactPost> UserReactPosts { get; set; } = new List<UserReactPost>();

    public virtual ICollection<UserRequestGroup> UserRequestGroups { get; set; } = new List<UserRequestGroup>();

    public virtual ICollection<SurveyItem> Surveyitems { get; set; } = new List<SurveyItem>();
}
