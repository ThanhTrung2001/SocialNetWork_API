﻿using System;
using System.Collections.Generic;

namespace EnVietSocialNetWorkAPI.Models;

public partial class NotificationType
{
    public int Id { get; set; }

    public string TypeName { get; set; } = null!;

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
