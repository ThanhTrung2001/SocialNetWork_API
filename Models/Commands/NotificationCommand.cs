namespace EnVietSocialNetWorkAPI.Models.Commands
{
    public class CreateNotificationCommand
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int NotiType { get; set; }
        public Guid? DestinationId { get; set; }
        public DateTime StartedAt { get; set; } = DateTime.Now;
        public DateTime? EndedAt { get; set; }
        public string OrganizationName { get; set; }
    }
}
