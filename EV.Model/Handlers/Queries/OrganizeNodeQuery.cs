namespace EV.Model.Handlers.Queries
{
    public class OrganizeNodeQuery
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public string Department { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Phone_Number { get; set; } = null!;

        public string Address { get; set; } = null!;

        public string City { get; set; } = null!;

        public string Country { get; set; } = null!;

        public int Level { get; set; }

        public Guid? Parent_Id { get; set; }

        public int Employee_Count { get; set; }

        public List<OrganizeNodeQuery> Children_Nodes { get; set; } = new List<OrganizeNodeQuery>();
    }

    public class OrganizationUserQuery
    {
        public string Department { get; set; }
        public string Organization_Role { get; set; }
        public Guid User_Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Avatar { get; set; }
    }
}
