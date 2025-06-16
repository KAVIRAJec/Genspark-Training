using Microsoft.AspNetCore.SignalR;

namespace Freelance_Project.Misc;

public class NotificationHub : Hub
{
    public async Task SendClientNotification(string clientId, string message)
    {
        if (!string.IsNullOrEmpty(message))
            await Clients.User(clientId).SendAsync("ClientNotification", message);
    }
    public async Task SendFreelancerNotification(string freelancerId, string message)
    {
        if (!string.IsNullOrEmpty(message))
            await Clients.User(freelancerId).SendAsync("FreelancerNotification", message);
    }
    public async Task SendClientNotificationToAll(string message)
    {
        if (!string.IsNullOrEmpty(message))
            await Clients.All.SendAsync("ClientNotification", message);
    }
}