namespace EV.Model.Handlers.Commands
{
    public class CreateOrganizeNodeCommand
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Department { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Phone_Number { get; set; } = null!;

        public string Address { get; set; } = null!;

        public string City { get; set; } = null!;

        public string Country { get; set; } = null!;

        public int Level { get; set; }

        public Guid? Parent_Id { get; set; }
    }


    public class CreateOrganizationUserCommand
    {
        public Guid User_Id { get; set; }
        public string Organization_Role { get; set; }
    }

    public class EditOrganizationUserCommand
    {
        public Guid User_Id { get; set; }
        public string Organization_Role { get; set; }
    }

    public class DeleteOrganizationUserCommand
    {
        public Guid User_Id { get; set; }
    }
}
