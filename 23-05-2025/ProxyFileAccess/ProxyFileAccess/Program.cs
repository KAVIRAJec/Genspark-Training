
using ProxyFileAccess;
using ProxyFileAccess.Models;

public class Program
{
    public static void Main(string[] args)
    {
        while(true)
        {
            Console.WriteLine("===========Proxy File Access System===========");
            Console.WriteLine("Enter user details to access the file:");
            User user = new();
            user.GetUserInput();
            ProxyFile proxy = new(user);
            proxy.Read();
            Console.WriteLine("Press Cntrl+C to exit...");
        }
    }
}