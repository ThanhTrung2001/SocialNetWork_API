namespace EnVietSocialNetWorkAPI.Model.Commands
{
    public class CreateNotificationCommand
    {
        public Guid User_Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Noti_Type { get; set; }
        public Guid? Destination_Id { get; set; }
        public DateTime Started_At { get; set; } = DateTime.Now;
        public DateTime? Ended_At { get; set; }
        public string Organization_Name { get; set; }
    }
}
