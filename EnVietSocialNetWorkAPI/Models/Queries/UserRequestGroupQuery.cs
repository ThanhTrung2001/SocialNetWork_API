namespace EnVietSocialNetWorkAPI.Models.Queries
{
    public class RequestJoinGroupQuery
    {
        public Guid User_Id { get; set; }
        public Guid Group_Id { get; set; }
        public string Status { get; set; }
        public bool Is_Admin_Request { get; set; }
        public Guid? Admin_Id { get; set; } = null;
        public DateTime Created_At { get; set; }
        public DateTime Updated_At { get; set; }
        public bool Is_Deleted { get; set; }
    }
}
