
using EnVietSocialNetWorkAPI.Model.Commands;

namespace EnVietSocialNetWorkAPI.RealTime.Repositories
{
    public interface IChatRepository
    {
        Task CreateChatBox(List<Guid> users, CreateChatGroupCommand chatbox);
        Task AddUserToChatBox(Guid id, Guid userId);
        Task DeleteChatBox(Guid id);
        Task SaveMessage(CreateMessageCommand message, Guid groupId);
        Task DeleteMessage(Guid messageId);

    }
}
