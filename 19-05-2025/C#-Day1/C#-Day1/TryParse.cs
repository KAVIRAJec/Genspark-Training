using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__Day1
{
    internal class TryParseNumber
    {
        public static void GetNumbersFromUser()
        {
            int num1, num2;
            Console.WriteLine("Please enter the first number");
            while (!int.TryParse(Console.ReadLine(), out num1))
                Console.WriteLine("Invalid input. Please try again");
            Console.WriteLine($"The incremented number is {++num1}");
        }
        public static void Run()
        {
            GetNumbersFromUser();
        }
    }
}
