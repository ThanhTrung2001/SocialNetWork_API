namespace EnVietSocialNetWorkAPI.Models.Queries
{
    public class ReactTypeQuery
    {
        public int ReactTypeId { get; set; }
        public string ReactTypeName { get; set; }
        public Guid ReactUserId { get; set; }
        public string ReactFirstName { get; set; }
        public string ReactLastName { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class PostReactQuery : ReactTypeQuery
    {

    }

    public class CommentReactQuery : ReactTypeQuery
    {

    }

    public class MessageReactQuery : ReactTypeQuery
    {

    }
}
