using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__Day1
{
    public class LargestNumber
    {
        public static void Run()
        {
            int num1, num2;
            Console.Write("Enter the number 1: ");
            num1 = Program.GetNumberInput();

            Console.Write("Enter the number 2: ");
            //num2 = Convert.ToInt32(Console.ReadLine());
            num2 = Program.GetNumberInput();

            Console.WriteLine($"The Largest number is {Math.Max(num1, num2)}");
        }
    }
}
