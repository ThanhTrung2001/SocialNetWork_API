using Microsoft.AspNetCore.SignalR;

namespace EnVietSocialNetWorkAPI.RealTime
{
    public class ChatHub : Hub
    {

        public override async Task OnConnectedAsync()
        {
            await Clients.All.SendAsync("ReceiveMessage", $"{Context.ConnectionId} has joined the chat.");
        }

        public async Task JoinGlobalChat(string user)
        {
            await Clients.All.SendAsync("ReceiveMessage", $"{Context.ConnectionId}, {user} has joined the chat.");
        }


        public async Task JoinSpecificRoom(string user, string group)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, group);
            await Clients.Group(group).SendAsync("ReceiveMessage", $"{Context.ConnectionId}, {user} has joined the group {group}.");
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user,message);

        }

        public async Task SendMessageToSpecific(string userName, string group, string message)
        {
            await Clients.Groups(group).SendAsync("ReceiveMessage", userName, message);
            
        }

        public async Task LeaveRoom(string userName, Guid? chatRoomId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatRoomId.ToString());
            await Clients.Group(chatRoomId.ToString()).SendAsync("SendMessage", $"{Context.ConnectionId}, {userName} has leave the group {chatRoomId}.");
        }

        public override async Task OnDisconnectedAsync(Exception? e)
        {
            await Clients.All.SendAsync("Receive Message", $"{Context.ConnectionId} has left the group wth error {e}.");
        }
    }
}
