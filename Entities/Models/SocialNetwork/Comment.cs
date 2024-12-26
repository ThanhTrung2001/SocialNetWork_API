namespace EnVietSocialNetWorkAPI.Entities.Models.SocialNetwork
{
    public class Comment : BaseClass
    {
        public string Content { get; set; }
        public string MediaURL { get; set; }
        public Guid UserId { get; set; }
        public Guid PostId { get; set; }

        public virtual User User { get; set; }
        public virtual Post Post { get; set; }
        //public ICollection<CommentReact>? Reacts { get; set; }

    }
}
