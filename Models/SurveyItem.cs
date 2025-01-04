using System;
using System.Collections.Generic;

namespace EnVietSocialNetWorkAPI.Models;

public partial class SurveyItem
{
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public string OptionName { get; set; } = null!;

    public int Total { get; set; }

    public Guid SurveyId { get; set; }

    public virtual Survey Survey { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
