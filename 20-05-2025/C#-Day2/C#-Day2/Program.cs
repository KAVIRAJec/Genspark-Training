using C__Day2;
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
    public static string GetStringInput()
    {
        string str;
        while (string.IsNullOrEmpty(str = Console.ReadLine()))
        {
            Console.WriteLine("Invalid Input. Please try again");
        }
        return str;
    }
    public static int[] Get1DArray(int size)
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
    public static void Main(string[] args)
    {
        //InstagramApp.Run();
        EmployeePromotion employeePromotion = new EmployeePromotion();
        employeePromotion.Run();
    }
}
