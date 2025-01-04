using System;
using System.Collections.Generic;

namespace EnVietSocialNetWorkAPI.Models;

public partial class OrganizeNode
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int FileType { get; set; }

    public int Level { get; set; }

    public Guid? ParentId { get; set; }

    public int EmployeeCount { get; set; }
}
