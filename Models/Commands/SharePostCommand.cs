namespace EnVietSocialNetWorkAPI.Model.Commands
{
    public class CreateSharePostCommand
    {
        public Guid SharedByUserId { get; set; }
        public string Content { get; set; }
        public bool InGroup { get; set; }
        public Guid? DestinationId { get; set; }
    }

    public class EditSharePostCommand
    {
        public string ShareContent { get; set; }
        public Guid SharedByUserId { get; set; }
    }
}
