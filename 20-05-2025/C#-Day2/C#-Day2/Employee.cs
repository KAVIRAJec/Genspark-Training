using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__Day2
{
    class Employee : IComparable<Employee>
    {
        int id, age;
        string name;
        double salary;
        public Employee()
        {
        }
        public Employee(int id, int age, string name, double salary)
        {
            this.id = id;
            this.age = age;
            this.name = name;
            this.salary = salary;
        }
        public void TakeEmployeeDetailsFromUser()
        {
            Console.Write("Please enter the employee ID: ");
            while (true)
            {
                id = Program.GetNumberInput();
                if(id <= 0)
                {
                    Console.WriteLine("ID Cannot be negative or Zero");
                    Console.Write("Please enter the employee ID: ");
                }
                else break;
            }
            Console.Write("Please enter the employee name: ");
            while (true)
            {
                name = Program.GetStringInput();
                if (name.Length < 3 || name.Length > 15)
                {
                    Console.WriteLine("Name should only contain 3 to 14 characters");
                    Console.Write("Please enter the employee name: ");
                }
                else break;
            }
            Console.Write("Please enter the employee age: ");
            while (true)
            {
                age = Program.GetNumberInput();
                if(age < 18 || age > 60)
                {
                    Console.WriteLine("Age should be between 18 and 60");
                    Console.Write("Please enter the employee age: ");
                }
                else break;

            }
            Console.Write("Please enter the employee salary: ");
            while (true)
            {
                salary = Program.GetNumberInput();
                if(salary < 0)
                {
                    Console.WriteLine("Salary cannot be negative");
                    Console.Write("Please enter the employee salary: ");
                }
                else break;
            }
        }
        public override string ToString()
        {
            return "Employee ID : " + id + "\tName : " + name + "\tAge : " + age + "\tSalary : " + salary;
        }
        public int Id { get => id; set => id = value; }
        public int Age { get => age; set => age = value; }
        public string Name { get => name; set => name = value; }
        public double Salary { get => salary; set => salary = value; }

        public int CompareTo(Employee other)
        {
            if (this.salary > other.salary)
                return 1;
            else if (this.salary < other.salary)
                return -1;
            else
                return 0;
        }
    }
}
