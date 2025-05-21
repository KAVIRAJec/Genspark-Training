using C__Day3.Interfaces;
using C__Day3.Models;
using C__Day3.Repositories;
using C__Day3.Services;

namespace C__Day3
{
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
        static void Main(string[] args)
        {
            IRepository<int, Employee> employeeRepository = new EmployeeRepository();
            IEmployeeService employeeService = new EmployeeService(employeeRepository);
            ManageEmployee manageEmployee = new ManageEmployee(employeeService);
            manageEmployee.Start();
        }
    }
}
