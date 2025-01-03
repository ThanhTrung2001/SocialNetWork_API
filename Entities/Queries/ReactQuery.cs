using EnVietSocialNetWorkAPI.Entities.Models.SocialNetwork;

namespace EnVietSocialNetWorkAPI.Entities.Queries
{
    public class ReactQuery
    {
        public Guid ReactId { get; set; }
        public ReactType Type { get; set; }
        public Guid ReactUserId { get; set; }
        public string ReactUserName { get; set; }
        public string ReactUserAvatar { get; set; }
    }

    public class CommentReactQuery : ReactQuery
    {

    }

    public class MessageReactQuery : ReactQuery
    {

    }
}
