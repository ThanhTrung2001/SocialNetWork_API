namespace EnVietSocialNetWorkAPI.Models.Commands
{
    public class CreateOrganizeNodeCommand
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Level { get; set; }
        public Guid? ParentId { get; set; }
    }
}
