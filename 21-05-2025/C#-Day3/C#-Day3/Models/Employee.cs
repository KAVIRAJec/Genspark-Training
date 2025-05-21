using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C__Day3.Models
{
    public class Employee: IComparable<Employee>, IEquatable<Employee>
    {
        public int Id { get; set; }
        public int Age { get; set; }
        public string Name { get; set; }
        public double Salary { get; set; }
        public Employee()
        {
            Name = string.Empty;
        }

        public Employee(int id, int age, string name, double salary)
        {
            Id = id;
            Age = age;
            Name = name;
            Salary = salary;
        }

        public void TakeEmployeeDetailsFromUser()
        {
            Console.Write("Please enter the employee ID: ");
            while (true)
            {
                Id = Program.GetNumberInput();
                if (Id <= 0)
                {
                    Console.WriteLine("ID Cannot be negative or Zero");
                    Console.Write("Please enter the employee ID: ");
                }
                else break;
            }
            Console.Write("Please enter the employee name: ");
            while (true)
            {
                Name = Program.GetStringInput();
                if (Name.Length < 3 || Name.Length > 15)
                {
                    Console.WriteLine("Name should only contain 3 to 14 characters");
                    Console.Write("Please enter the employee name: ");
                }
                else break;
            }
            Console.Write("Please enter the employee age: ");
            while (true)
            {
                Age = Program.GetNumberInput();
                if (Age < 18 || Age > 60)
                {
                    Console.WriteLine("Age should be between 18 and 60");
                    Console.Write("Please enter the employee age: ");
                }
                else break;

            }
            Console.Write("Please enter the employee salary: ");
            while (true)
            {
                Salary = Program.GetNumberInput();
                if (Salary < 0)
                {
                    Console.WriteLine("Salary cannot be negative");
                    Console.Write("Please enter the employee salary: ");
                }
                else break;
            }
        }
        public override string ToString()
        {
            return "Employee ID : " + Id + "\tName : " + Name + "\tAge : " + Age + "\tSalary : " + Salary;
        }
        public int CompareTo(Employee? other)
        {
            return this.Id.CompareTo(other?.Id);
        }

        public bool Equals(Employee? other)
        {
           return this.Id == other?.Id;
        }
    }
}
