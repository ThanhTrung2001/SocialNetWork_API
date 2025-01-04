using EnVietSocialNetWorkAPI.Entities.Commands;

namespace EnVietSocialNetWorkAPI.RealTime.Repositories
{
    public interface IChatRepository
    {
        Task CreateChatBox(List<Guid> users, NewChatGroup chatbox);
        Task AddUserToChatBox(Guid id, Guid userId);
        Task DeleteChatBox(Guid id);
        Task SaveMessage(NewMessage message, Guid groupId);
        Task DeleteMessage(Guid messageId);

    }
}
