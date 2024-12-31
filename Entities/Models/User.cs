namespace EnVietSocialNetWorkAPI.Entities.Models
{
    public class User : BaseClass
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string AvatarUrl { get; set; }
        public UserRole Role { get; set; }

        //public ICollection<Post> Posts { get; set; }
        //public ICollection<Comment> Comment { get; set; }
        //public ICollection<React> Reacts { get; set; }
        ////public ICollection<CommentReact> CommentReacts { get; set; }
        ////public ICollection<MessageReact> MessageReacts { get; set; }
        //public ICollection<Message> Messages { get; set; }

        //public ICollection<ChatBox>? ChatBoxes { get; set; }
        //public ICollection<UserChatBox>? UserChatBoxes { get; set; } = [];
        //public ICollection<SurveyItem> SurveyItems { get; set; }
    }

    public enum UserRole
    {
        CEO = 1,
        Admin = 2,
        EMployee = 3
    }
}
