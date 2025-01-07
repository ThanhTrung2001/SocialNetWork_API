namespace EnVietSocialNetWorkAPI.Model.Queries
{
    public class ReactQuery
    {
        public int ReactId { get; set; }
        public string TypeName { get; set; }
        public Guid ReactUserId { get; set; }
        public string ReactFirstName { get; set; }
        public string ReactLastName { get; set; }
        public string ReactAvatar { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class PostReactQuery : ReactQuery
    {

    }

    public class CommentReactQuery : ReactQuery
    {

    }

    public class MessageReactQuery : ReactQuery
    {

    }
}
