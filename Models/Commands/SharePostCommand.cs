namespace EnVietSocialNetWorkAPI.Model.Commands
{
    public class CreateSharePostCommand
    {
        public Guid Shared_By_User_Id { get; set; }
        public string Content { get; set; }
        public bool In_Group { get; set; }
        public Guid? Destination_Id { get; set; }
    }

    public class EditSharePostCommand
    {
        public string Share_Content { get; set; }
        public Guid Shared_By_User_Id { get; set; }
    }
}
