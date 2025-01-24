namespace EV.Model.Handlers.Commands
{
    public class RequestJoinGroupCommand
    {
        public Guid User_Id { get; set; }
        public Guid Group_Id { get; set; }
    }

    public class ModifyRequestJoinCommand
    {
        public Guid User_Id { get; set; }
        public Guid Group_Id { get; set; }
        public string Status { get; set; }
    }

    public class AdminRecommendCommand
    {
        public Guid User_Id { get; set; }
        public Guid Group_Id { get; set; }
        public Guid Admin_Id { get; set; }
    }
}
