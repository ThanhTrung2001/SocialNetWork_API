using EnVietSocialNetWorkAPI.Entities.Models.SocialNetwork;

namespace EnVietSocialNetWorkAPI.Entities.Queries
{
    public class NotificationQuery
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string OrganizeName { get; set; }
        public Guid DestinationID { get; set; }
        public NotificationType NotiType { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }

    }
}
