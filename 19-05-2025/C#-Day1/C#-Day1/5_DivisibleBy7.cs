using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__Day1
{
    public class DivisibleBy7
    {
        public static void Run()
        {
            int totalNumbers = 10;
            int count = CountDivisibleBySeven(totalNumbers);

            Console.WriteLine($"Total numbers divisible by 7: {count}");
        }
        public static int ReadNumber(int index)
        {
            Console.Write($"Enter number {index}: ");
            return Convert.ToInt32(Console.ReadLine());
        }
        public static bool IsDivisibleBySeven(int number)
        {
            return number % 7 == 0;
        }
        public static int CountDivisibleBySeven(int totalNumbers)
        {
            int count = 0;
            for (int i = 1; i <= totalNumbers; i++)
            {
                int num = ReadNumber(i);
                if (IsDivisibleBySeven(num))
                {
                    count++;
                }
            }
            return count;
        }
    }
}