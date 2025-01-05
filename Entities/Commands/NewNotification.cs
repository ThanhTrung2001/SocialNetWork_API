using EnVietSocialNetWorkAPI.Entities.Models.SocialNetwork;

namespace EnVietSocialNetWorkAPI.Entities.Commands
{
    public class NotificationCommand
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public NotificationType Type { get; set; }
        public Guid? DestinationId { get; set; }
        public DateTime StartedAt { get; set; } = DateTime.Now;
        public DateTime EndedAt { get; set; } = DateTime.Now;
        public string OrganizeName { get; set; }
    }
}
