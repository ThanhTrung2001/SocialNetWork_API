using EV.DataAccess.Repositories;

namespace EV.DataAccess.UnitOfWorks.Interface
{
    public interface IUnitOfWork
    {
        UserRepository UserRepository { get; }
        UploadFileRepository UploadFileRepository { get; }
        TagRepository TagRepository { get; }
        SurveyItemRepository SurveyItemRepository { get; }
        SurveyRepository SurveyRepository { get; }
        SurveyVoteRepository SurveyVoteRepository { get; }
        SharePostRepository SharePostRepository { get; }
        ReactRepository ReactRepository { get; }
        PostRepository PostRepository { get; }
        PageRepository PageRepository { get; }
        OrganizationRepository OrganizationRepository { get; }
        NotificationRepository NotificationRepository { get; }
        MessageRepository MessageRepository { get; }
        JoinGroupRequestRepository JoinGroupRequestRepository { get; }
        GroupRepository GroupRepository { get; }
        CommentRepository CommentRepository { get; }
        ChatGroupRepository ChatGroupRepository { get; }
        AttachmentRepository AttachmentRepository { get; }
    }
}
