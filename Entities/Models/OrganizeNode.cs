namespace EnVietSocialNetWorkAPI.Entities.Models
{
    public class OrganizeNode
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int FileType { get; set; }
        public int Level { get; set; }
        public Guid? ParentId { get; set; }
        public List<OrganizeNode> ChildrenNodes { get; set; } = new List<OrganizeNode>();
    }
}
