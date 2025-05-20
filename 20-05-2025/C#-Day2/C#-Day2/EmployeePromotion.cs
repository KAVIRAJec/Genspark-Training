using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace C__Day2
{
    internal class EmployeePromotion
    {
        private List<Employee> employees = new List<Employee>();
        private Dictionary<int,Employee> employeeDetails = new Dictionary<int, Employee>();
        private static int employeeId = 1;

        public void CollectEmployeeNames()
        {
            Console.WriteLine("Please enter the employee names in the order of their eligibility for promotion (Enter blank to stop):");

            while (true)
            {
                string name = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(name))
                {
                    break;
                }
                Employee employee = new Employee();
                employee.Name = name;
                employees.Add(employee);
            }
            Console.WriteLine($"The current size of the collection is: {employees.Capacity}");
            employees.TrimExcess();
            Console.WriteLine($"The size after removing the extra space is: {employees.Capacity}");
        }
        public void DisplayPromotions()
        {
            Console.WriteLine("Eligible Employees for Promotion:");
            if(employees.Count == 0)
            {
                Console.WriteLine("No employees eligible for promotion.");
                return;
            }
            int rank = 1;
            foreach (var employee in employees)
            {
                Console.WriteLine($"{rank++}: {employee.Name}");
            }
        }
        public void FindEmployeePromotion()
        {
            Console.Write("Please enter the name of the employee to check promotion position: ");
            string name = Program.GetStringInput();
            int position = employees.FindIndex(e => e.Name.Equals(name));
            if (position != -1)
            {
                Console.WriteLine($"\"{name}\" is the position {position+1} for promotion.");
            }
        }
        public void DisplayEmployees()
        {
            Console.WriteLine("Promoted employee list:");
            var sortedEmployees = employees.OrderBy(e => e.Name).ToList();
            foreach (var employee in sortedEmployees)
            {
                Console.WriteLine(employee.Name);
            }
        }
        //-----------------------------------Medium-----------------------------------
        public void AddEmployeeDetails()
        {
            Employee emp = new Employee();
            emp.TakeEmployeeDetailsFromUser();

            while(employeeDetails.ContainsKey(emp.Id))
            {
                Console.WriteLine("Employee ID already exists. Please enter a unique ID.");
                emp.Id = Program.GetNumberInput();
            }
            employeeDetails.Add(emp.Id, emp);
            Console.WriteLine("Employee Created added successfully.");
        }
        public void SortEmployeeBySalary()
        {
            var sortedEmployees = employeeDetails.Values.ToList();
            sortedEmployees.Sort();
            if (sortedEmployees.Count == 0)
            {
                Console.WriteLine("No employees to display.");
                return;
            }else 
                Console.WriteLine("Employees sorted by salary:");
            foreach (var employee in sortedEmployees)
                {
                    Console.Write(employee.ToString());
                }
        }
        public void FindEmployeeById()
        {
            Console.Write("Please enter the employee ID: ");
            int id = Program.GetNumberInput();

            var employee = employeeDetails.ToList().Where(emp => emp.Key == id).FirstOrDefault();

            if (employee.Value != null)
                Console.WriteLine(employee.Value.ToString());
            else
                Console.WriteLine("Employee not found.");
        }
        public void FindEmployeeByName()
        {
            Console.Write("Please enter the employee name: ");
            String name = Program.GetStringInput();

            var employee = employeeDetails.ToList().Where(emp => emp.Value.Name.Equals(name)).FirstOrDefault();

            if (employee.Value != null)
                Console.WriteLine(employee.Value.ToString());
            else
                Console.WriteLine("Employee not found.");
        }
        public void FindEmployeeElderThan()
        {
            Console.Write("Please enter the employee ID to compare: ");
            int id = Program.GetNumberInput();
            var employee = employeeDetails.ToList().Where(emp => emp.Key == id).FirstOrDefault();
            if (employee.Value != null)
            {
                Console.WriteLine("Employees older than " + employee.Value.Name + "(" + employee.Value.Age + ")" + ":");
                bool found = false;
                foreach (var emp in employeeDetails.Values)
                {
                    if (emp.Age > employee.Value.Age)
                    {
                        Console.WriteLine(emp.ToString());
                        found = true;
                    }
                }
                if (!found)
                    Console.WriteLine("No employees older than " + employee.Value.Name + "(" + employee.Value.Age + ")" + " found.");
            }
            else
                Console.WriteLine("Employee not found with id: " + id);
        }
        //-------------------------Hard-----------------------------------
        public void DisplayAllEmployees()
        {
            Console.WriteLine("All Employees Details:");
            bool found = false;
            foreach (var employee in employeeDetails.Values)
            {
                found = true;
                Console.WriteLine(employee.ToString());
            }
            if (!found)
                Console.WriteLine("No employees found.");
        }
        public void UpdateEmployeeDetails()
        {
            Console.WriteLine("Please enter the employee ID to update: ");
            int id = Program.GetNumberInput();
            var employee = employeeDetails.ToList().Where(emp => emp.Key == id).FirstOrDefault();
            if (employee.Value != null)
            {
                Console.WriteLine("Current Employee Details:");
                Console.WriteLine(employee.Value.ToString());
                Console.WriteLine();
                Console.Write("Please enter the employee name: ");
                while (true)
                {
                    employee.Value.Name = Program.GetStringInput();
                    if (employee.Value.Name.Length < 3 || employee.Value.Name.Length > 15)
                    {
                        Console.WriteLine("Name should only contain 3 to 14 characters");
                        Console.Write("Please enter the employee name: ");
                    }
                    else break;
                }
                Console.Write("Please enter the employee age: ");
                while (true)
                {
                    employee.Value.Age = Program.GetNumberInput();
                    if (employee.Value.Age < 18 || employee.Value.Age > 60)
                    {
                        Console.WriteLine("Age should be between 18 and 60");
                        Console.Write("Please enter the employee age: ");
                    }
                    else break;
                }
                Console.Write("Please enter the employee salary: ");
                while (true)
                {
                    employee.Value.Salary = Program.GetNumberInput();
                    if (employee.Value.Salary < 0)
                    {
                        Console.WriteLine("Salary cannot be negative");
                        Console.Write("Please enter the employee salary: ");
                    }
                    else break;
                }
                Console.WriteLine("Employee details updated successfully.");
            }
            else
                Console.WriteLine("Employee not found with id: " + id);
        }
        public void DeleteEmployeeDetails()
        {
            Console.Write("Enter the Employee ID to delete: ");
            int id = Program.GetNumberInput();
            if (employeeDetails.ContainsKey(id))
            {
                employeeDetails.Remove(id);
                Console.WriteLine("Employee with ID " + id + " deleted successfully.");
            }
            else
            {
                Console.WriteLine("Employee with ID " + id + " not found.");
            }
        }
        public void Run()
        {
            try
            {
                while (true)
                {
                    Console.WriteLine("-----Employee Promotion Management System-----");
                    Console.WriteLine("-Easy(Employee Promotion)");
                    Console.WriteLine("1-Enter employee for promotion");
                    Console.WriteLine("2-Display employee Promotion");
                    Console.WriteLine("3-Find Employee Promotion");
                    Console.WriteLine("4-Display Employees in ascending");
                    Console.WriteLine("-Medium(Employee Filtering)");
                    Console.WriteLine("5-Filter Employee Details");
                    Console.WriteLine("-Hard(Employee CRUD)");
                    Console.WriteLine("6-Perform CRUD");
                    Console.WriteLine("7-Exit");
                    int choice = Program.GetNumberInput();
                    switch (choice)
                    {
                        case 1:
                            CollectEmployeeNames();
                            break;
                        case 2:
                            DisplayPromotions();
                            break;
                        case 3:
                            FindEmployeePromotion();
                            break;
                        case 4:
                            DisplayEmployees();
                            break;
                        case 5:
                            Console.WriteLine();
                            Console.WriteLine("----Employee Filtering System-----");
                            Console.WriteLine("1-Sort Employee by Salary");
                            Console.WriteLine("2-Get Employee by ID");
                            Console.WriteLine("3-Get Employee by Name");
                            Console.WriteLine("4-Find employee Elder than a employee");
                            Console.WriteLine("5-Exit");

                            int filterChoice = Program.GetNumberInput();
                            switch(filterChoice)
                            {
                                case 1:
                                    SortEmployeeBySalary();
                                    break;
                                case 2:
                                    FindEmployeeById();
                                    break;
                                case 3:
                                    FindEmployeeByName();
                                    break;
                                case 4:
                                    FindEmployeeElderThan();
                                    break;
                                case 5:
                                    break;
                                default:
                                    Console.WriteLine("Invalid choice. Please try again.");
                                    break;
                            }
                            break;
                        case 6:
                            Console.WriteLine("------CRUD Operations------");
                            Console.WriteLine("1-Create Employee");
                            Console.WriteLine("2-Display all Employee");
                            Console.WriteLine("3-Display Employee by ID");
                            Console.WriteLine("4-Update Employee");
                            Console.WriteLine("5-Delete Employee");
                            Console.WriteLine("6-Exit");

                            int crudChoice = Program.GetNumberInput();
                            switch(crudChoice)
                            {
                                case 1:
                                    AddEmployeeDetails();
                                    break;
                                case 2:
                                    DisplayAllEmployees();
                                    break;
                                case 3:
                                    FindEmployeeById();
                                    break;
                                case 4:
                                    UpdateEmployeeDetails();
                                    break;
                                case 5:
                                    DeleteEmployeeDetails();
                                    break;
                                case 6: 
                                    break;
                                default:
                                    Console.WriteLine("Invalid choice. Please try again.");
                                    break;
                            }
                            break;
                        case 7:
                            return;
                        default:
                            Console.WriteLine("Invalid choice. Please try again.");
                            break;
                    }
                    Console.WriteLine("--------------------------------------");
                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }
    }
}
