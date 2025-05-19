using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__Day1
{
    internal class SudokuRow
    {
        public static bool ValidateSudokuRow(int[] arr)
        {
            for(int i=1; i<=9;i++)
            {
                if (!arr.Contains(i))
                {
                    return false;
                }
            }
            return true;
        }
        public static void Run()
        {
            Console.WriteLine("Enter the Array Values: ");
            int[] arr = Program.GetArray(9);

            if (ValidateSudokuRow(arr))
                Console.WriteLine("Valid Sudoku Row");
            else
                Console.WriteLine("Invalid Sudoku Row");
        }
    }
}
