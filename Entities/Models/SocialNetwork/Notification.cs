namespace EnVietSocialNetWorkAPI.Entities.Models.SocialNetwork
{
    public class Notification : BaseClass
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public NotificationType NotiType { get; set; }
        public Guid? DestinationId { get; set; }
    }

    public enum NotificationType
    {
        Warning = 1,
        Information,
        HappyBirthday,
        System,
        Calendar
    }
}
