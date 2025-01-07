using Microsoft.EntityFrameworkCore;

namespace EnVietSocialNetWorkAPI.Models;

public partial class SocialNetworkDbContext : DbContext
{
    public SocialNetworkDbContext()
    {
    }

    public SocialNetworkDbContext(DbContextOptions<SocialNetworkDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Attachment> Attachments { get; set; }

    public virtual DbSet<ChatGroup> ChatGroups { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Organization> Organizations { get; set; }

    public virtual DbSet<Page> Pages { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<PostType> PostTypes { get; set; }

    public virtual DbSet<SharePost> SharePosts { get; set; }

    public virtual DbSet<Survey> Surveys { get; set; }

    public virtual DbSet<SurveyItem> SurveyItems { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserChatgroup> UserChatgroups { get; set; }

    public virtual DbSet<UserDetail> UserDetails { get; set; }

    public virtual DbSet<UserGroup> UserGroups { get; set; }

    public virtual DbSet<UserOrganization> UserOrganizations { get; set; }

    public virtual DbSet<UserPage> UserPages { get; set; }

    public virtual DbSet<UserReactComment> UserReactComments { get; set; }

    public virtual DbSet<UserReactMessage> UserReactMessages { get; set; }

    public virtual DbSet<UserReactPost> UserReactPosts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("server=DESKTOP-M3OV5QE\\SQLEXPRESS; database=SocialNetworkDB; Integrated Security=true; Encrypt=false; TrustServerCertificate=True; Trusted_Connection=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Attachment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__attachme__3213E83FAF0F4708");

            entity.ToTable("attachments");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasMaxLength(100)
                .HasColumnName("description");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.Media)
                .HasMaxLength(255)
                .HasColumnName("media");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<ChatGroup>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__chat_gro__3213E83F6ED80443");

            entity.ToTable("chat_groups");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.GroupType)
                .HasMaxLength(50)
                .HasColumnName("group_type");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Theme)
                .HasMaxLength(255)
                .HasColumnName("theme");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__comments__3213E83F5865EAD3");

            entity.ToTable("comments");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.IsResponse).HasColumnName("is_response");
            entity.Property(e => e.IsSharepost).HasColumnName("is_sharepost");
            entity.Property(e => e.PostId).HasColumnName("post_id");
            entity.Property(e => e.ReactCount).HasColumnName("react_count");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__comments__user_i__0D44F85C");

            entity.HasMany(d => d.Attachments).WithMany(p => p.Comments)
                .UsingEntity<Dictionary<string, object>>(
                    "CommentAttachment",
                    r => r.HasOne<Attachment>().WithMany()
                        .HasForeignKey("AttachmentId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__comment_a__attac__01D345B0"),
                    l => l.HasOne<Comment>().WithMany()
                        .HasForeignKey("CommentId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__comment_a__comme__00DF2177"),
                    j =>
                    {
                        j.HasKey("CommentId", "AttachmentId").HasName("PK__comment___DCE1A9C98ECBAE1D");
                        j.ToTable("comment_attachment");
                        j.IndexerProperty<Guid>("CommentId").HasColumnName("comment_id");
                        j.IndexerProperty<Guid>("AttachmentId").HasColumnName("attachment_id");
                    });

            entity.HasMany(d => d.Comments).WithMany(p => p.Responses)
                .UsingEntity<Dictionary<string, object>>(
                    "CommentResponse",
                    r => r.HasOne<Comment>().WithMany()
                        .HasForeignKey("CommentId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__comment_r__comme__1B9317B3"),
                    l => l.HasOne<Comment>().WithMany()
                        .HasForeignKey("ResponseId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__comment_r__respo__1C873BEC"),
                    j =>
                    {
                        j.HasKey("CommentId", "ResponseId").HasName("PK__comment___992BBB0EAE741ED9");
                        j.ToTable("comment_response");
                        j.IndexerProperty<Guid>("CommentId").HasColumnName("comment_id");
                        j.IndexerProperty<Guid>("ResponseId").HasColumnName("response_id");
                    });

            entity.HasMany(d => d.Responses).WithMany(p => p.Comments)
                .UsingEntity<Dictionary<string, object>>(
                    "CommentResponse",
                    r => r.HasOne<Comment>().WithMany()
                        .HasForeignKey("ResponseId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__comment_r__respo__1C873BEC"),
                    l => l.HasOne<Comment>().WithMany()
                        .HasForeignKey("CommentId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__comment_r__comme__1B9317B3"),
                    j =>
                    {
                        j.HasKey("CommentId", "ResponseId").HasName("PK__comment___992BBB0EAE741ED9");
                        j.ToTable("comment_response");
                        j.IndexerProperty<Guid>("CommentId").HasColumnName("comment_id");
                        j.IndexerProperty<Guid>("ResponseId").HasColumnName("response_id");
                    });
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__groups__3213E83F22FF0C22");

            entity.ToTable("groups");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.Avatar)
                .HasMaxLength(255)
                .HasColumnName("avatar");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.Wallpaper)
                .HasMaxLength(255)
                .HasColumnName("wallpaper");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__messages__3213E83F7CFDD8D8");

            entity.ToTable("messages");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.ChatgroupId).HasColumnName("chatgroup_id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.IsPinned).HasColumnName("is_pinned");
            entity.Property(e => e.IsReponse).HasColumnName("is_reponse");
            entity.Property(e => e.ReactCount).HasColumnName("react_count");
            entity.Property(e => e.SenderId).HasColumnName("sender_id");
            entity.Property(e => e.Status)
                .HasMaxLength(255)
                .HasColumnName("status");
            entity.Property(e => e.Type)
                .HasMaxLength(255)
                .HasColumnName("type");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Chatgroup).WithMany(p => p.Messages)
                .HasForeignKey(d => d.ChatgroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__messages__chatgr__1A9EF37A");

            entity.HasOne(d => d.Sender).WithMany(p => p.Messages)
                .HasForeignKey(d => d.SenderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__messages__sender__19AACF41");

            entity.HasMany(d => d.Attachments).WithMany(p => p.Messages)
                .UsingEntity<Dictionary<string, object>>(
                    "MessageAttachment",
                    r => r.HasOne<Attachment>().WithMany()
                        .HasForeignKey("AttachmentId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__message_a__attac__03BB8E22"),
                    l => l.HasOne<Message>().WithMany()
                        .HasForeignKey("MessageId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__message_a__messa__02C769E9"),
                    j =>
                    {
                        j.HasKey("MessageId", "AttachmentId").HasName("PK__message___30CBB1A84465EBFA");
                        j.ToTable("message_attachment");
                        j.IndexerProperty<Guid>("MessageId").HasColumnName("message_id");
                        j.IndexerProperty<Guid>("AttachmentId").HasColumnName("attachment_id");
                    });

            entity.HasMany(d => d.Messages).WithMany(p => p.Responses)
                .UsingEntity<Dictionary<string, object>>(
                    "MessageResponse",
                    r => r.HasOne<Message>().WithMany()
                        .HasForeignKey("MessageId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__message_r__messa__1D7B6025"),
                    l => l.HasOne<Message>().WithMany()
                        .HasForeignKey("ResponseId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__message_r__respo__1E6F845E"),
                    j =>
                    {
                        j.HasKey("MessageId", "ResponseId").HasName("PK__message___7501A36F2279ACF1");
                        j.ToTable("message_response");
                        j.IndexerProperty<Guid>("MessageId").HasColumnName("message_id");
                        j.IndexerProperty<Guid>("ResponseId").HasColumnName("response_id");
                    });

            entity.HasMany(d => d.Responses).WithMany(p => p.Messages)
                .UsingEntity<Dictionary<string, object>>(
                    "MessageResponse",
                    r => r.HasOne<Message>().WithMany()
                        .HasForeignKey("ResponseId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__message_r__respo__1E6F845E"),
                    l => l.HasOne<Message>().WithMany()
                        .HasForeignKey("MessageId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__message_r__messa__1D7B6025"),
                    j =>
                    {
                        j.HasKey("MessageId", "ResponseId").HasName("PK__message___7501A36F2279ACF1");
                        j.ToTable("message_response");
                        j.IndexerProperty<Guid>("MessageId").HasColumnName("message_id");
                        j.IndexerProperty<Guid>("ResponseId").HasColumnName("response_id");
                    });
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__notifica__3213E83F10EFD8C2");

            entity.ToTable("notifications");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.DestinationId).HasColumnName("destination_id");
            entity.Property(e => e.Endedat)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("endedat");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.NotiType).HasColumnName("noti_type");
            entity.Property(e => e.OrganizationName)
                .HasMaxLength(255)
                .HasColumnName("organization_name");
            entity.Property(e => e.Startedat)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("startedat");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__notificat__user___18B6AB08");

            entity.HasMany(d => d.Tags).WithMany(p => p.Notifications)
                .UsingEntity<Dictionary<string, object>>(
                    "NotificationTag",
                    r => r.HasOne<Tag>().WithMany()
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__notificat__tag_i__29E1370A"),
                    l => l.HasOne<Notification>().WithMany()
                        .HasForeignKey("NotificationId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__notificat__notif__28ED12D1"),
                    j =>
                    {
                        j.HasKey("NotificationId", "TagId").HasName("PK__notifica__9470EE0497D5F181");
                        j.ToTable("notification_tag");
                        j.IndexerProperty<Guid>("NotificationId").HasColumnName("notification_id");
                        j.IndexerProperty<int>("TagId").HasColumnName("tag_id");
                    });
        });

        modelBuilder.Entity<Organization>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__organiza__3213E83FEF3A39D6");

            entity.ToTable("organizations");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.City)
                .HasMaxLength(100)
                .HasColumnName("city");
            entity.Property(e => e.Country)
                .HasMaxLength(100)
                .HasColumnName("country");
            entity.Property(e => e.Department)
                .HasMaxLength(255)
                .HasColumnName("department");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .HasColumnName("description");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.EmployeeCount).HasColumnName("employee_count");
            entity.Property(e => e.Level).HasColumnName("level");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Parentid).HasColumnName("parentid");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .HasColumnName("phone_number");
        });

        modelBuilder.Entity<Page>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__pages__3213E83F1178E806");

            entity.ToTable("pages");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.Avatar)
                .HasMaxLength(255)
                .HasColumnName("avatar");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.Wallpaper)
                .HasMaxLength(255)
                .HasColumnName("wallpaper");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__posts__3213E83FE82D5DC3");

            entity.ToTable("posts");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DestinationId).HasColumnName("destination_id");
            entity.Property(e => e.InGroup).HasColumnName("in_group");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.PostTypeId).HasColumnName("post_type_id");
            entity.Property(e => e.ReactCount).HasColumnName("react_count");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.PostType).WithMany(p => p.Posts)
                .HasForeignKey(d => d.PostTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__posts__post_type__0C50D423");

            entity.HasMany(d => d.Attachments).WithMany(p => p.Posts)
                .UsingEntity<Dictionary<string, object>>(
                    "PostAttachment",
                    r => r.HasOne<Attachment>().WithMany()
                        .HasForeignKey("Attachmentid")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__post_atta__attac__2BC97F7C"),
                    l => l.HasOne<Post>().WithMany()
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__post_atta__post___2AD55B43"),
                    j =>
                    {
                        j.HasKey("PostId", "Attachmentid").HasName("PK__post_att__B296ECFD3E23C7C4");
                        j.ToTable("post_attachment");
                        j.IndexerProperty<Guid>("PostId").HasColumnName("post_id");
                        j.IndexerProperty<Guid>("Attachmentid").HasColumnName("attachmentid");
                    });

            entity.HasMany(d => d.Tags).WithMany(p => p.Posts)
                .UsingEntity<Dictionary<string, object>>(
                    "PostTag",
                    r => r.HasOne<Tag>().WithMany()
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__post_tag__tag_id__27F8EE98"),
                    l => l.HasOne<Post>().WithMany()
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__post_tag__post_i__2704CA5F"),
                    j =>
                    {
                        j.HasKey("PostId", "TagId").HasName("PK__post_tag__4AFEED4D693B1CF6");
                        j.ToTable("post_tag");
                        j.IndexerProperty<Guid>("PostId").HasColumnName("post_id");
                        j.IndexerProperty<int>("TagId").HasColumnName("tag_id");
                    });
        });

        modelBuilder.Entity<PostType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__post_typ__3213E83F551EECEB");

            entity.ToTable("post_types");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<SharePost>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__share_po__3213E83FAB9CA0A2");

            entity.ToTable("share_posts");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DestinationId).HasColumnName("destination_id");
            entity.Property(e => e.InGroup).HasColumnName("in_group");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.ReactCount).HasColumnName("react_count");
            entity.Property(e => e.SharedByUserId).HasColumnName("shared_by_user_id");
            entity.Property(e => e.SharedPostId).HasColumnName("shared_post_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.SharedByUser).WithMany(p => p.SharePosts)
                .HasForeignKey(d => d.SharedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__share_pos__share__17C286CF");

            entity.HasOne(d => d.SharedPost).WithMany(p => p.SharePosts)
                .HasForeignKey(d => d.SharedPostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__share_pos__share__16CE6296");
        });

        modelBuilder.Entity<Survey>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__surveys__3213E83F3B5D41CD");

            entity.ToTable("surveys");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.ExpiredAt)
                .HasColumnType("datetime")
                .HasColumnName("expired_at");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.PostId).HasColumnName("post_id");
            entity.Property(e => e.SurveyTypeId).HasColumnName("survey_type_id");
            entity.Property(e => e.TotalVote).HasColumnName("total_vote");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Post).WithMany(p => p.Surveys)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__surveys__post_id__11158940");
        });

        modelBuilder.Entity<SurveyItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__survey_i__3213E83F1113BD11");

            entity.ToTable("survey_items");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.OptionName).HasColumnName("option_name");
            entity.Property(e => e.SurveyId).HasColumnName("survey_id");
            entity.Property(e => e.TotalVote).HasColumnName("total_vote");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Survey).WithMany(p => p.SurveyItems)
                .HasForeignKey(d => d.SurveyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__survey_it__surve__15DA3E5D");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tags__3213E83F2CDDD446");

            entity.ToTable("tags");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.TagName)
                .HasMaxLength(100)
                .HasColumnName("tag_name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__users__3213E83FCC831D65");

            entity.ToTable("users");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .HasColumnName("password");
            entity.Property(e => e.Role)
                .HasMaxLength(255)
                .HasColumnName("role");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .HasColumnName("username");

            entity.HasMany(d => d.Surveyitems).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserSurveyitemVote",
                    r => r.HasOne<SurveyItem>().WithMany()
                        .HasForeignKey("SurveyitemId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__user_surv__surve__2057CCD0"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__user_surv__user___1F63A897"),
                    j =>
                    {
                        j.HasKey("UserId", "SurveyitemId").HasName("PK__user_sur__61574B0CE5314756");
                        j.ToTable("user_surveyitem_vote");
                        j.IndexerProperty<Guid>("UserId").HasColumnName("user_id");
                        j.IndexerProperty<Guid>("SurveyitemId").HasColumnName("surveyitem_id");
                    });
        });

        modelBuilder.Entity<UserChatgroup>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.ChatgroupId }).HasName("PK__user_cha__E594A209042EEED3");

            entity.ToTable("user_chatgroup");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.ChatgroupId).HasColumnName("chatgroup_id");
            entity.Property(e => e.DelayUntil)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("delay_until");
            entity.Property(e => e.IsNotNotification).HasColumnName("is_not_notification");
            entity.Property(e => e.Role)
                .HasMaxLength(255)
                .HasColumnName("role");

            entity.HasOne(d => d.Chatgroup).WithMany(p => p.UserChatgroups)
                .HasForeignKey(d => d.ChatgroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__user_chat__chatg__2610A626");

            entity.HasOne(d => d.User).WithMany(p => p.UserChatgroups)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__user_chat__user___251C81ED");
        });

        modelBuilder.Entity<UserDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__user_det__3213E83FBC4E6CAC");

            entity.ToTable("user_details");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.Avatar)
                .HasMaxLength(255)
                .HasColumnName("avatar");
            entity.Property(e => e.Bio)
                .HasMaxLength(100)
                .HasColumnName("bio");
            entity.Property(e => e.City)
                .HasMaxLength(100)
                .HasColumnName("city");
            entity.Property(e => e.Country)
                .HasMaxLength(100)
                .HasColumnName("country");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Dob)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("dob");
            entity.Property(e => e.Firstname)
                .HasMaxLength(100)
                .HasColumnName("firstname");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.Lastname)
                .HasMaxLength(100)
                .HasColumnName("lastname");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .HasColumnName("phone_number");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Wallpaper)
                .HasMaxLength(255)
                .HasColumnName("wallpaper");

            entity.HasOne(d => d.User).WithMany(p => p.UserDetails)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__user_deta__user___0B5CAFEA");
        });

        modelBuilder.Entity<UserGroup>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.GroupId }).HasName("PK__user_gro__A4E94E55155DF607");

            entity.ToTable("user_group");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.GroupId).HasColumnName("group_id");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.IsFollow).HasColumnName("is_follow");
            entity.Property(e => e.JoinedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("joined_at");
            entity.Property(e => e.Role)
                .HasMaxLength(255)
                .HasColumnName("role");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Group).WithMany(p => p.UserGroups)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__user_grou__group__22401542");

            entity.HasOne(d => d.User).WithMany(p => p.UserGroups)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__user_grou__user___214BF109");
        });

        modelBuilder.Entity<UserOrganization>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.NodeId }).HasName("PK__user_org__CC4FA9FE9A94EE19");

            entity.ToTable("user_organization");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.NodeId).HasColumnName("node_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.OrganizationRole)
                .HasMaxLength(255)
                .HasColumnName("organization_role");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Node).WithMany(p => p.UserOrganizations)
                .HasForeignKey(d => d.NodeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__user_orga__node___0A688BB1");

            entity.HasOne(d => d.User).WithMany(p => p.UserOrganizations)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__user_orga__user___09746778");
        });

        modelBuilder.Entity<UserPage>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.PageId }).HasName("PK__user_pag__0F89C40A91E25183");

            entity.ToTable("user_page");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.PageId).HasColumnName("page_id");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.IsFollow).HasColumnName("is_follow");
            entity.Property(e => e.JoinedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("joined_at");
            entity.Property(e => e.Role)
                .HasMaxLength(255)
                .HasColumnName("role");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Page).WithMany(p => p.UserPages)
                .HasForeignKey(d => d.PageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__user_page__page___24285DB4");

            entity.HasOne(d => d.User).WithMany(p => p.UserPages)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__user_page__user___2334397B");
        });

        modelBuilder.Entity<UserReactComment>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.Commentid }).HasName("PK__user_rea__F564B3B34C1BA7F2");

            entity.ToTable("user_react_comment");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Commentid).HasColumnName("commentid");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.ReactType)
                .HasMaxLength(255)
                .HasColumnName("react_type");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Comment).WithMany(p => p.UserReactComments)
                .HasForeignKey(d => d.Commentid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__user_reac__comme__0697FACD");

            entity.HasOne(d => d.User).WithMany(p => p.UserReactComments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__user_reac__user___05A3D694");
        });

        modelBuilder.Entity<UserReactMessage>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.MessageId }).HasName("PK__user_rea__C905C1E175E548C3");

            entity.ToTable("user_react_message");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.MessageId).HasColumnName("message_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.ReactType)
                .HasMaxLength(255)
                .HasColumnName("react_type");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Message).WithMany(p => p.UserReactMessages)
                .HasForeignKey(d => d.MessageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__user_reac__messa__0880433F");

            entity.HasOne(d => d.User).WithMany(p => p.UserReactMessages)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__user_reac__user___078C1F06");
        });

        modelBuilder.Entity<UserReactPost>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.PostId }).HasName("PK__user_rea__CA534F7965EFE179");

            entity.ToTable("user_react_post");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.PostId).HasColumnName("post_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.IsSharepost).HasColumnName("is_sharepost");
            entity.Property(e => e.ReactType)
                .HasMaxLength(255)
                .HasColumnName("react_type");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.User).WithMany(p => p.UserReactPosts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__user_reac__user___04AFB25B");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
