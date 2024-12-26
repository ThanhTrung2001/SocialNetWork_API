namespace EnVietSocialNetWorkAPI.Entities.Models.SocialNetwork.Chat
{
    public class ChatBox : BaseClass
    {
        public string BoxName { get; set; }
        public string BoxType { get; set; } //1-1 or group chat
        public string Theme { get; set; }
        public ICollection<User>? Users { get; set; } = [];
        public ICollection<Message>? Messages { get; set; } = [];
        public ICollection<UserChatBox>? UserChatBoxes { get; set; } = [];
    }

    public class UserChatBox
    {
        public Guid UserId;
        public Guid ChatBoxId;
        public User User { get; set; }
        public ChatBox ChatBox { get; set; }
    }
}
