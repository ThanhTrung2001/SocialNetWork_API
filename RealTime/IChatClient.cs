namespace EnVietSocialNetWorkAPI.RealTime
{
    public interface IChatClient
    {
        Task ReceiveMessage(string message);
    }
}
