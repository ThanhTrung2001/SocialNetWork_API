namespace EnVietSocialNetWorkAPI.Model.Queries
{
    public class ReactQuery
    {
        public string React_Type { get; set; }
        public Guid React_UserId { get; set; }
        public string React_FirstName { get; set; }
        public string React_LastName { get; set; }
        public string React_Avatar { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class PostReactQuery : ReactQuery
    {
        public bool Is_SharePost { get; set; }
    }

    public class CommentReactQuery : ReactQuery
    {
        public bool Is_SharePost { get; set; }
    }

    public class MessageReactQuery : ReactQuery
    {

    }
}
