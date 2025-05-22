using C__Day3.Interfaces;
using C__Day3.Models;
using C__Day3.Repositories;
using C__Day3.Services;
using C__Day3.Misc;

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
            //IRepository<int, Employee> employeeRepository = new EmployeeRepository();
            //IEmployeeService employeeService = new EmployeeService(employeeRepository);
            //ManageEmployee manageEmployee = new ManageEmployee(employeeService);
            //manageEmployee.Start();
            //new Program();
            //Program program = new();
            //program.FindEmployee();
            //program.SortEmployee();
            string str = "Studen";
            Console.WriteLine(str.StringValidationCheck());
        }

        // Deligates 
        List<Employee> employees = new List<Employee>()
            {
            new Employee(101, 21, "John Doe", 50000),
            new Employee(102, 21, "Jane Smith", 60000),
            new Employee(103, 21,"Bob Johnson", 55000)
        };
        //public delegate void MyDelegate<T>(T num1, T num2);
        //public delegate void MyFDelegate(float num1, float num2);
        public void Add(int n1, int n2)
        {
            int sum = n1 + n2;
            Console.WriteLine($"The sum of {n1} and {n2} is {sum}");
        }
        public void Product(int n1, int n2)
        {
            int prod = n1 * n2;
            Console.WriteLine($"The sum of {n1} and {n2} is {prod}");
        }
        Program()
        {
            //MyDelegate<int> del = new MyDelegate<int>(Add);
            Action<int, int> del = Add;
            del += Product;
            // Anonymous method
            //del += delegate (int n1, int n2)
            //{
            //    Console.WriteLine($"The division result of {n1} and {n2} is {n1 / n2}");
            //};
            // Lambda expression
            del += (int n1, int n2) => Console.WriteLine($"The division result of {n1} and {n2} is {n1 / n2}");
            del(100, 20);
        }
        void FindEmployee()
        {
            int empId = 102;
            Predicate<Employee> predicate = e => e.Id == empId;
            Employee? emp = employees.Find(predicate);
            Console.WriteLine(emp.ToString() ?? "Employee not found");
        }
        void SortEmployee()
        {
            var sortedEmployees = employees.OrderBy(e => e.Name);
            foreach (var emp in sortedEmployees)
            {
                Console.WriteLine(emp.ToString());
            }
        }
    }
}
