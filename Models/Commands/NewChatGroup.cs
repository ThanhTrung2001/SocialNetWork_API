namespace EnVietSocialNetWorkAPI.Models.Commands
{
    public class NewChatGroup
    {
        //public Guid ChatBoxId { get; set; }
        public string BoxName { get; set; }
        public string BoxType { get; set; } //1-1 or group
        public string Theme { get; set; } = "normal";
        public List<Guid> Users { get; set; } = new List<Guid>();
    }
}
