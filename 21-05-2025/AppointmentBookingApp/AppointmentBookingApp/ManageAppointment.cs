using AppointmentBookingApp.Interfaces;
using AppointmentBookingApp.Models;
using AppointmentBookingApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentBookingApp
{
    public class ManageAppointment
    {
        private readonly IAppointmentService _appointmentService;
        public ManageAppointment(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }
        public void Start()
        {
            while (true)
            {
                ShowMenu();
                int choice = Program.GetIntInput();
                while(choice < 1 || choice > 3)
                {
                    Console.Write("Invalid choice. Please select a valid option: ");
                    choice = Program.GetIntInput();
                }
                switch (choice)
                {
                    case 1:
                        BookAppointment();
                        break;
                    case 2:
                        SearchAppointment();
                        break;
                    case 3:
                        Console.WriteLine("Exiting the system. Goodbye!");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
                Console.WriteLine();
            }
        }
        public void ShowMenu()
        {
            Console.WriteLine("===========Appointment Management System===========");
            Console.WriteLine("1. Book Appointment");
            Console.WriteLine("2. Search Appointment");
            Console.WriteLine("3. Exit");
            Console.Write("Select an option: ");
        }
        public void BookAppointment()
        {
            Appointment appointment = new Appointment();
            appointment.GetInputFromUser();
            int id = _appointmentService.BookAppointment(appointment);
            Console.WriteLine(id != -1 ? $"Appointment booked successfully with ID: {id}" : "");
        }
        public void SearchAppointment()
        {
            var searchOption = GetSearchOption();
            var appointments = _appointmentService.SearchAppointment(searchOption);
            if (appointments != null && appointments.Count > 0)
            {
                Console.WriteLine("Search Results:");
                foreach (var appointment in appointments)
                {
                    Console.WriteLine(appointment);
                }
            } else
            {
                Console.WriteLine("No appointments found matching the search criteria.");
            }
        }
        public SearchModel GetSearchOption()
        {
            Console.WriteLine("Choose Search criteria:");
            Console.WriteLine("1. Patient Name");
            Console.WriteLine("2. Appointment Date");
            Console.WriteLine("3. Patient Age Range");
            
            int choice = Program.GetIntInput();
            while (choice < 1 || choice > 3)
            {
                Console.Write("Invalid choice. Please select a valid option: ");
                choice = Program.GetIntInput();
            }
            SearchModel searchModel = new SearchModel();
            switch (choice)
            {
                case 1:
                    Console.Write("Enter Patient Name: ");
                    while (true)
                    {
                        searchModel.PatientName = Program.GetStringInput();
                        if (searchModel.PatientName.Length <= 2 || searchModel.PatientName.Length > 12)
                            Console.Write("Patient Name must be 3 to 12 characters. Please enter a valid name: ");
                        else break;
                    }
                    break;
                case 2:
                    Console.Write("Enter Appointment Date (DD/MM/YYYY hh:mm AM/PM): ");
                    while (true)
                    {
                        searchModel.AppointmentDate = Program.GetDateTimeInput();
                        if (searchModel.AppointmentDate < DateTime.Now)
                            Console.Write("Appointment Date cannot be in the past. Please enter a valid date: ");
                        else break;
                    }
                    break;
                case 3:

                    Console.Write("Enter minimum Patient Age: ");
                    while (true)
                    {
                        searchModel.AgeRange = new AgeRange<int>();
                        searchModel.AgeRange.MinAge = Program.GetIntInput();
                        if (searchModel.AgeRange.MinAge < 1 || searchModel.AgeRange.MinAge > 100)
                            Console.Write("Patient Age must be between 1 and 100. Please enter a valid age: ");
                        else break;
                    }
                    Console.Write("Enter maximum Patient Age: ");
                    while (true)
                    {
                        searchModel.AgeRange.MaxAge = Program.GetIntInput();
                        if (searchModel.AgeRange.MaxAge < 1 || searchModel.AgeRange.MaxAge > 100)
                            Console.Write("Patient Age must be between 1 and 100. Please enter a valid age: ");
                        else break;
                    }
                    break;
            }
            return searchModel;
        }
    }
}
