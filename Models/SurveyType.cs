﻿using System;
using System.Collections.Generic;

namespace EnVietSocialNetWorkAPI.Models;

public partial class SurveyType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Survey> Surveys { get; set; } = new List<Survey>();
}
