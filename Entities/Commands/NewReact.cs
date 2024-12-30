using EnVietSocialNetWorkAPI.Entities.Models.SocialNetwork;

namespace EnVietSocialNetWorkAPI.Entities.Commands
{
    public class NewReact
    {
        public Guid PostId { get; set; }
        public ReactType ReactType { get; set; } = ReactType.Like;
        public Guid UserId { get; set; }
    }
}
