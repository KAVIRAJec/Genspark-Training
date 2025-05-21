using AppointmentBookingApp;
using AppointmentBookingApp.Interfaces;
using AppointmentBookingApp.Models;
using AppointmentBookingApp.Repositories;
using AppointmentBookingApp.Services;
using System.Globalization;

public class Program
{
    public static void Main(string[] args)
    {
        IRepository<int, Appointment> appointmentRepository = new AppointmentRepository();
        IAppointmentService appointmentService = new AppointmentService(appointmentRepository);
        ManageAppointment manageAppointment = new ManageAppointment(appointmentService);
        manageAppointment.Start();
    }
    public static string GetStringInput()
    {
        string str;
        while(string.IsNullOrEmpty(str = Console.ReadLine()))
        {
            Console.WriteLine("Invalid Input. Please try again");
        }
        return str;
    }
    public static int GetIntInput()
    {
        int num;
        while(!int.TryParse(Console.ReadLine(), out num))
        {
            Console.WriteLine("Invalid Input. Please try again");
        }
        return num;
    }
    public static DateTime GetDateTimeInput()
    {
        DateTime date;
        string format = "dd/MM/yyyy hh:mm tt";
        while (!DateTime.TryParseExact(Console.ReadLine(),format,
                                   CultureInfo.InvariantCulture,
                                   DateTimeStyles.None, out date))
        {
            Console.WriteLine("Invalid Input. Please try again");
        }
        return date;
    }
}