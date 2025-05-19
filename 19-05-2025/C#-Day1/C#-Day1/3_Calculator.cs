using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__Day1
{
    internal class Calculator
    {
        public static void Run()
        {
            int num1, num2;
            Console.Write("Enter the number 1: ");
            num1 = Convert.ToInt32(Console.ReadLine());

            Console.Write("Enter the number 2: ");
            num2 = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Enter the operations to perform: 1 +, 2 -, 3 *,4 / :");

            int opr = Convert.ToInt32(Console.ReadLine());
            switch (opr)
            {
                case 1:
                    Console.WriteLine($"Sum: {num1 + num2}");
                    break;
                case 2:
                    Console.WriteLine($"Difference: {num1 - num2}");
                    break;
                case 3:
                    Console.WriteLine($"Product: {num1 * num2}");
                    break;
                case 4:
                    if (num2 != 0)
                        Console.WriteLine($"Quotient: {num1 / num2}");
                    else
                        Console.WriteLine("Error: Division by zero");
                    break;
                default:
                    Console.WriteLine("Invalid Operations");
                    break;
            }
        }
    }
}
