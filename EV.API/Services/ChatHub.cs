﻿using Microsoft.AspNetCore.SignalR;

namespace EnVietSocialNetWorkAPI.Services
{
    public class ChatHub : Hub
    {

        public ChatHub()
        {
        }

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
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task SendMessageToSpecific(Guid userName, Guid group, string message)
        {
            await Clients.Groups(group.ToString()).SendAsync("ReceiveMessage", userName, message);

        }

        public async Task LeaveRoom(string userName, string chatRoomId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatRoomId);
            await Clients.Group(chatRoomId).SendAsync("SendMessage", $"{Context.ConnectionId}, {userName} has leave the group {chatRoomId}.");
        }

        public override async Task OnDisconnectedAsync(Exception? e)
        {
            await Clients.All.SendAsync("Receive Message", $"{Context.ConnectionId} has left the group wth error {e}.");
        }
    }
}