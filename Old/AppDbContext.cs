//namespace EnVietSocialNetWorkAPI.DataConnection
//{
//    using EnVietSocialNetWorkAPI.Entities;
//    using EnVietSocialNetWorkAPI.Entities.Models;
//    using EnVietSocialNetWorkAPI.Entities.Models.SocialNetwork;
//    using EnVietSocialNetWorkAPI.Entities.Models.SocialNetwork.Chat;
//    using Microsoft.EntityFrameworkCore;

//    public class AppDbContext : DbContext
//    {
//        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

//        public DbSet<User> Users { get; set; }
//        public DbSet<SurveyItem> SurveyItems { get; set; }
//        public DbSet<Survey> Surveys { get; set; }
//        public DbSet<React> Reacts { get; set; }
//        //public DbSet<CommentReact> CommentReacts { get; set; }
//        //public DbSet<MessageReact> MessageReacts { get; set; }
//        public DbSet<Post> Posts { get; set; }
//        public DbSet<Message> Messages { get; set; }
//        public DbSet<ChatBox> ChatBoxes { get; set; }
//        public DbSet<Notification> Notifications { get; set; }
//        public DbSet<Comment> Comments { get; set; }
//        public DbSet<Group> Groups { get; set; }
//        public DbSet<MediaItem> MediaItems { get; set; }
//    }

//}
