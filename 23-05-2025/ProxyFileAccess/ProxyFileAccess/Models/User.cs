using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyFileAccess.Models
{
    public class User
    {
        public string Username { get; set; }
        public UserRole Role { get; set; }

        public void GetUserInput()
        {
            Console.WriteLine("Enter username:");
            Username = Console.ReadLine();
            while (string.IsNullOrEmpty(Username))
            {
                Console.WriteLine("Username cannot be empty. Please enter a valid username:");
                Username = Console.ReadLine();
            }
            Console.WriteLine("Enter role (Admin, User, Guest):");
            string roleInput = Console.ReadLine();
            UserRole parsedRole;
            while (!Enum.TryParse(roleInput, true, out parsedRole))
            {
                Console.WriteLine("Invalid role. Please enter a valid role (Admin, User, Guest):");
                roleInput = Console.ReadLine();
            }
            Role = parsedRole;
        }
        public string ToString()
        {
            return $"User: {Username}, Role: {Role}";
        }
    }
    public enum UserRole
    {
        Admin,
        User,
        Guest
    }
}