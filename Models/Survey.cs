using System;
using System.Collections.Generic;

namespace EnVietSocialNetWorkAPI.Models;

public partial class Survey
{
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime ExpiredAt { get; set; }

    public int Total { get; set; }

    public int SurveyTypeId { get; set; }

    public Guid? PostId { get; set; }

    public string? Question { get; set; }

    public virtual Post? Post { get; set; }

    public virtual ICollection<SurveyItem> SurveyItems { get; set; } = new List<SurveyItem>();

    public virtual SurveyType SurveyType { get; set; } = null!;
}
