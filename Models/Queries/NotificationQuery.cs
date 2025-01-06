namespace EnVietSocialNetWorkAPI.Models.Queries
{
    public class NotificationQuery
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string OrganizationName { get; set; }
        public Guid DestinationID { get; set; }
        public int NotiType { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }

    }
}
