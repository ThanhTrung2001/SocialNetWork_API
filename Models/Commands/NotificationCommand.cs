namespace EnVietSocialNetWorkAPI.Models.Commands
{
    public class CreateNotificationCommand
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public NotificationType Type { get; set; }
        public Guid? DestinationId { get; set; }
        public string OrganizeName { get; set; }
        public DateTime StartedAt { get; set; } = DateTime.Now;
        public DateTime? EndedAt { get; set; }
    }
}
