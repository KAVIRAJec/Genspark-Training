using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__Day1
{
    internal class FrequencyCount
    {
        public static void PrintFrequency(Dictionary<int, int> frequencyMap)
        {
            Console.WriteLine("The frequency of each number:");
            foreach (var num in frequencyMap)
            {
                Console.WriteLine($"Number {num.Key} occurs {num.Value} times.");
            }
        }
        private static Dictionary<int,int> CountFrequency(int[] arr, Dictionary<int, int> frequencyMap)
        {
            foreach (int num in arr)
            {
                if (frequencyMap.ContainsKey(num))
                {
                    frequencyMap[num]++;
                }
                else
                {
                    frequencyMap[num] = 1;
                }
            }
            return frequencyMap;
        }
        public static void Run()
        {
            Console.Write("Please enter the size of the array: ");
            int size = Program.GetNumberInput();

            int[] arr = Program.GetArray(size);

            Dictionary<int, int> frequencyMap = new Dictionary<int, int>();

            CountFrequency(arr, frequencyMap);
            PrintFrequency(frequencyMap);
        }
    }
}
