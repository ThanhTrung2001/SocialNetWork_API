namespace EnVietSocialNetWorkAPI.Entities.Commands
{
    public class OrganizeNodeCommand
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int FileType { get; set; }
        public int Level { get; set; }
        public Guid ParentId { get; set; }
    }
}
