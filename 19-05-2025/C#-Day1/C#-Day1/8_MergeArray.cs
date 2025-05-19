using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__Day1
{
    internal class MergeArray
    {
        private static int[] MergeArrays(int[] arr1, int[] arr2)
        {
            int size1 = arr1.Length;
            int size2 = arr2.Length;
            int[] mergedArr = new int[size1 + size2];
            int i = 0, j = 0, k = 0;
            while (i < size1 && j < size2)
            {
                if (arr1[i] < arr2[j])
                {
                    mergedArr[k++] = arr1[i++];
                }
                else
                {
                    mergedArr[k++] = arr2[j++];
                }
            }
            while (i < size1)
            {
                mergedArr[k++] = arr1[i++];
            }
            while (j < size2)
            {
                mergedArr[k++] = arr2[j++];
            }
            return mergedArr;
        }
        public static void Run()
        {
            Console.Write("Enter the size of the first array: ");
            int size1 = Program.GetNumberInput();
            int[] arr1 = Program.GetArray(size1);
            Console.Write("Enter the size of the second array: ");
            int size2 = Program.GetNumberInput();
            int[] arr2 = Program.GetArray(size2);
            int[] mergedArr = MergeArrays(arr1, arr2);
            Rotation.PrintArr(mergedArr);
        }
    }
}
