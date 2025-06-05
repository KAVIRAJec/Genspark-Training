using Microsoft.AspNetCore.SignalR;
using FileApp.Models;

namespace FileApp.Misc
{
    public class NotificationHub : Hub
    {
        public async Task SendMessage(string user, FileModel file)
        {
            var message = $"{user} has uploaded a new file: {file.FileName} ({file.Size})";
            await Clients.All.SendAsync("ReceiveMessage", message);
        }
    }
}