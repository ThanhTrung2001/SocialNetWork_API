using EV.Common.Services.Email;
using EV.DataAccess.DataConnection;
using EV.DataAccess.Repositories;
using EV.DataAccess.UnitOfWorks.Interface;

namespace EV.DataAccess.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DatabaseContext _context;
        private readonly IEmailHandler _emailHandler;
        public UnitOfWork(DatabaseContext context, IEmailHandler emailHandler)
        {
            _context = context;
            _emailHandler = emailHandler;
        }

        public UserRepository UserRepository => new Lazy<UserRepository>(() => new UserRepository(_context, _emailHandler)).Value;

        public UploadFileRepository UploadFileRepository => new Lazy<UploadFileRepository>(() => new UploadFileRepository(_context)).Value;

        public TagRepository TagRepository => new Lazy<TagRepository>(() => new TagRepository(_context)).Value;

        public SurveyItemRepository SurveyItemRepository => new Lazy<SurveyItemRepository>(() => new SurveyItemRepository(_context)).Value;

        public SurveyRepository SurveyRepository => new Lazy<SurveyRepository>(() => new SurveyRepository(_context)).Value;

        public SurveyVoteRepository SurveyVoteRepository => new Lazy<SurveyVoteRepository>(() => new SurveyVoteRepository(_context)).Value;

        public SharePostRepository SharePostRepository => new Lazy<SharePostRepository>(() => new SharePostRepository(_context)).Value;

        public ReactRepository ReactRepository => new Lazy<ReactRepository>(() => new ReactRepository(_context)).Value;

        public PostRepository PostRepository => new Lazy<PostRepository>(() => new PostRepository(_context)).Value;

        public PageRepository PageRepository => new Lazy<PageRepository>(() => new PageRepository(_context)).Value;

        public OrganizationRepository OrganizationRepository => new Lazy<OrganizationRepository>(() => new OrganizationRepository(_context)).Value;

        public NotificationRepository NotificationRepository => new Lazy<NotificationRepository>(() => new NotificationRepository(_context)).Value;

        public MessageRepository MessageRepository => new Lazy<MessageRepository>(() => new MessageRepository(_context)).Value;

        public JoinGroupRequestRepository JoinGroupRequestRepository => new Lazy<JoinGroupRequestRepository>(() => new JoinGroupRequestRepository(_context)).Value;

        public GroupRepository GroupRepository => new Lazy<GroupRepository>(() => new GroupRepository(_context)).Value;

        public CommentRepository CommentRepository => new Lazy<CommentRepository>(() => new CommentRepository(_context)).Value;

        public ChatGroupRepository ChatGroupRepository => new Lazy<ChatGroupRepository>(() => new ChatGroupRepository(_context)).Value;

        public AttachmentRepository AttachmentRepository => new Lazy<AttachmentRepository>(() => new AttachmentRepository(_context)).Value;
    }
}