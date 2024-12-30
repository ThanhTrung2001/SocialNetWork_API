namespace EnVietSocialNetWorkAPI.Entities.Commands
{
    public class NewChatBox
    {
        //public Guid ChatBoxId { get; set; }
        public string BoxName { get; set; }
        public string BoxType { get; set; } //1-1 or group chat
        public string Theme { get; set; } = "normal";
        public List<Guid> Users { get; set; } = new List<Guid>();
    }
}
