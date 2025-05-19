using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__Day1
{
    internal class ValidateSudokuGame
    {
        public static bool ValidateAllColumns(int[,] board)
        {
            for (int col = 0; col < 9; col++)
            {
                int[] currentCol = new int[9];
                for (int row = 0; row < 9; row++)
                {
                    currentCol[row] = board[row, col];
                }

                if (!SudokuRow.ValidateSudokuRow(currentCol))
                {
                    Console.WriteLine($"Invalid column at index {col + 1}");
                    return false;
                }
            }
            return true;
        }
        public static bool ValidateAllSubgrids(int[,] board)
        {
            for (int startRow = 0; startRow < 9; startRow += 3)
            {
                for (int startCol = 0; startCol < 9; startCol += 3)
                {
                    int[] subgrid = new int[9];
                    int index = 0;

                    for (int row = startRow; row < startRow + 3; row++)
                    {
                        for (int col = startCol; col < startCol + 3; col++)
                        {
                            subgrid[index++] = board[row, col];
                        }
                    }

                    if (!SudokuRow.ValidateSudokuRow(subgrid))
                    {
                        Console.WriteLine($"Invalid subgrid starting at ({startRow + 1},{startCol + 1})");
                        return false;
                    }
                }
            }
            return true;
        }
        public static bool ValidateAllRows(int[,] board)
        {
            for (int row = 0; row < 9; row++)
            {
                int[] currentRow = new int[9];
                for (int col = 0; col < 9; col++)
                {
                    currentRow[col] = board[row, col];
                }

                if (!SudokuRow.ValidateSudokuRow(currentRow))
                {
                    Console.WriteLine($"Invalid row at index {row + 1}");
                    return false;
                }
            }
            return true;
        }
        public static void Run()
        {
            try
            {
                int[,] board = new int[9, 9];

                Console.WriteLine("Enter Sudoku board (9 rows, each with 9 numbers):");
                for (int i = 0; i < 9; i++)
                {
                    Console.WriteLine($"Enter 9 numbers for row {i + 1}, separated by spaces:");
                    string[] input = Console.ReadLine().Split(' ');

                    while (input.Length != 9 || !input.All(x => int.TryParse(x, out _)))
                    {
                        Console.WriteLine("Invalid input. Please enter exactly 9 integers separated by spaces:");
                        input = Console.ReadLine().Split(' ');
                    }

                    for (int j = 0; j < 9; j++)
                    {
                        board[i, j] = int.Parse(input[j]);
                    }
                }

                bool rowsValid = ValidateAllRows(board);
                bool colsValid = ValidateAllColumns(board);
                bool subgridsValid = ValidateAllSubgrids(board);

                if (rowsValid && colsValid && subgridsValid)
                    Console.WriteLine("All Sudoku rows are valid.");
                else
                    Console.WriteLine("Sudoku board has invalid row(s).");
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }
    }
}
