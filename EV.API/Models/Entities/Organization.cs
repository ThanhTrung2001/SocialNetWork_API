using System;
using System.Collections.Generic;

namespace EnVietSocialNetWorkAPI.Models.Entities;

public partial class Organization
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string Department { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string City { get; set; } = null!;

    public string Country { get; set; } = null!;

    public int Level { get; set; }

    public Guid? ParentId { get; set; }

    public int EmployeeCount { get; set; }

    public virtual ICollection<UserOrganization> UserOrganizations { get; set; } = new List<UserOrganization>();
}
