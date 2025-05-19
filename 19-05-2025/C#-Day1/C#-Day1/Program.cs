using C__Day1;
using System;

public class Program
{
    public static int GetNumberInput()
    {
        int num;
        while (!int.TryParse(Console.ReadLine(), out num))
        {
            Console.WriteLine("Invalid Input. Please try again");
        }
        return num;
    }
    public static int[] GetArray(int size)
    {
        int[] arr = new int[size];
        for (int i = 0; i < size; i++)
        {
            Console.Write($"Please enter the {i + 1} number: ");
            int num = GetNumberInput();
            arr[i] = num;
        }
        return arr;
    }
    public static string GetStringInput()
    {
        string str;
        while (string.IsNullOrEmpty(str = Console.ReadLine()))
        {
            Console.WriteLine("Invalid Input. Please try again");
        }
        return str;
    }
    public static void Main(string[] args)
    {
        //TryParseNumber.Run();     
        //Greetings.Run();           // 1
        //LargestNumber.Run();       // 2
        //Calculator.Run();          // 3
        //Credential_Validator.Run();// 4
        //DivisibleBy7.Run();        // 5
        //FrequencyCount.Run();      // 6
        //Rotation.Run();            // 7
        //MergeArray.Run();          // 8
        //Secret_Word.Run();         // 9
        //SudokuRow.Run();           // 10
        //ValidateSudokuGame.Run();  // 11
        StringASCIEncryption.Run();  // 12
    }
}
