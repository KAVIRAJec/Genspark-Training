using Freelance_Project.Contexts;
using Microsoft.AspNetCore.SignalR;

public class CustomUserIdProvider : IUserIdProvider
{
    public string GetUserId(HubConnectionContext connection)
    {
        var userId = connection.User?.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
        Console.WriteLine($"CustomUserIdProvider: UserId is {userId}");
        return userId;
    }
}