using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__Day1
{
    internal class StringASCIEncryption
    {
        static int asciiShift = 3;
        private static void EncryptString(string input)
        {
            StringBuilder encyptedString = new StringBuilder(input.Length);
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i]>= 97 && input[i]<=122)
                encyptedString.Append((char)((input[i] + asciiShift)>122 ? input[i] + asciiShift- 26 : input[i] + asciiShift));
            }
            Console.WriteLine($"Encrypted String: {encyptedString.ToString()}");
        }
        private static void DecryptString(string input)
        {
            StringBuilder decryptedString = new StringBuilder(input.Length);
            for(int i = 0; i < input.Length; i++)
            {
                if (input[i] >= 97 && input[i] <= 122)
                decryptedString.Append((char)((input[i] - asciiShift)<97 ? input[i] - asciiShift + 26 : input[i] + asciiShift));
            }
            Console.WriteLine($"Decrypted String: {decryptedString.ToString()}");
        }
        public static void Run()
        {
            Console.WriteLine("ASCII Based String Encryption");
            try
            {
                while (true)
                {
                    Console.WriteLine("===================================");
                    Console.WriteLine("Press 1 to encrypt a string");
                    Console.WriteLine("Press 2 to decrypt a string");
                    Console.WriteLine("Press 3 to Change ASCII Shift value");
                    Console.WriteLine("Press 4 to exit");

                    Console.Write("Enter your choice: ");
                    int choice = Program.GetNumberInput();

                    switch (choice)
                    {
                        case 1:
                            Console.Write("Enter a string to encrypt: ");
                            String input = Program.GetStringInput();
                            EncryptString(input.ToLower());
                            break;
                        case 2:
                            Console.Write("Enter a string to decrypt: ");
                            String decryptInp = Program.GetStringInput();
                            DecryptString(decryptInp.ToLower());
                            break;
                        case 3:
                            Console.WriteLine("Current Shift Value: " + asciiShift);
                            Console.Write("Enter new shift value: ");
                            asciiShift = Program.GetNumberInput();
                            Console.WriteLine("Shift value updated to: " + asciiShift);
                            break;
                        case 4:
                            Console.WriteLine("Exiting...");
                            return;
                        default:
                            Console.WriteLine("Invalid choice. Please try again.");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
            finally
            {
                Console.WriteLine("Thank you for using the ASCII Encryption program!!");

            }
        }
    }
}
