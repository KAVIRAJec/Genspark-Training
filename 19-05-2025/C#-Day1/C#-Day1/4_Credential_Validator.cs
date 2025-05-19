using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__Day1
{
    public class Credential_Validator
    {
        private static bool ValidateCredentials(string username, string password)
        {
            if (String.Equals(username, "Admin") && String.Equals(password, "pass"))
            {
                return true;
            }
            return false;
        }
        public static void Run()
        {
            int attempt = 3;
            while (attempt > 0)
            {
                Console.Write("Enter Username: ");
                string username = Program.GetStringInput();
                Console.Write("Enter Password: ");
                string password = Program.GetStringInput();

                if (ValidateCredentials(username, password))
                {
                    Console.WriteLine("Username & Password Validation Successful");
                    return;
                }
                else
                {
                    attempt--;
                    Console.WriteLine($"Invalid Credentials! Attempts left: {attempt}");
                }
            }
            Console.WriteLine("No more Attempts left!");
        }
    }
}
