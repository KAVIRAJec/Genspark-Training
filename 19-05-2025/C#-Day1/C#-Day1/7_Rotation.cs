using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__Day1
{
    internal class Rotation
    {
        private static void LeftRotate(int[] arr, int value)
        {
            int size = arr.Length;
            int [] rotatedArr = new int[size];

            int ind = 0;
            for (int i=value; i<size; i++)
            {
                rotatedArr[ind++] = arr[i];
            }
            for (int i=0; i<value; i++)
            {
                rotatedArr[ind++] = arr[i];
            }
            PrintArr(rotatedArr);
        }
        public static void PrintArr(int[] arr)
        {
            Console.WriteLine("The rotated array is: ");
            foreach (int num in arr)
            {
                Console.Write(num + " ");
            }
            Console.WriteLine();
        }
        public static void Run()
        {
            Console.Write("Enter the array size: ");
            int size = Program.GetNumberInput();

            int[] arr = Program.GetArray(size);

            Console.Write("Enter the number of left rotations: ");
            int rotations = Program.GetNumberInput();

            if (rotations > 0)
            {
                LeftRotate(arr, rotations % size);
            }
            else
            {
                Console.WriteLine("No rotations needed.");
            }
        }
    }
}
