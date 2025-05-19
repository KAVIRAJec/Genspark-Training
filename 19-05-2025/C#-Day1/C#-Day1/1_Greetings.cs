using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__Day1
{
    internal class Greetings
    {
        public static void Run()
        {
            Console.Write("Enter your name here: ");
            string name = Console.ReadLine();
            Console.WriteLine($"Welcome to our Page, {name}!!");
        }
    }
}
