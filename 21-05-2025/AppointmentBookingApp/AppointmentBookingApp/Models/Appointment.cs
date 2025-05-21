using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppointmentBookingApp.Models
{
    public class Appointment: IComparable<Appointment>, IEquatable<Appointment>
    {
        public int Id { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public int PatientAge { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Reason { get; set; } = string.Empty;
        public Appointment() { }
        public Appointment(int id, string patientName, int patientAge, DateTime appointmentDate, string reason)
        {
            Id = id;
            PatientName = patientName;
            PatientAge = patientAge;
            AppointmentDate = appointmentDate;
            Reason = reason;
        }
        public void GetInputFromUser()
        {
            //Console.Write("Enter Patient Id: ");
            //while (true)
            //{
            //    Id = Program.GetIntInput();
            //    if (Id < 1)
            //        Console.Write("Id cannot be zero or negative. Please enter a valid Id: ");
            //    else break;
            //}
            Console.Write("Enter Patient Name: ");
            while (true)
            {
                PatientName = Program.GetStringInput();
                if (PatientName.Length <= 2 || PatientName.Length > 12)
                    Console.Write("Patient Name must be 3 to 12 characters. Please enter a valid name: ");
                else break;
            }
            Console.Write("Enter Patient Age: ");
            while (true)
            {
                PatientAge = Program.GetIntInput();
                if (PatientAge < 1 || PatientAge > 100)
                    Console.Write("Patient Age must be between 1 and 100. Please enter a valid age: ");
                else break;
            }
            Console.Write("Enter Appointment Date (DD/MM/YYYY hh:mm AM/PM): ");
            while (true)
            {
                AppointmentDate = Program.GetDateTimeInput();
                if (AppointmentDate < DateTime.Now) Console.Write("Appointment Date cannot be in the past. Please enter a valid date: ");
                else break;
            }
            Console.Write("Enter Appointment Reason: ");
            while (true)
            {
                Reason = Program.GetStringInput();
                if (String.IsNullOrWhiteSpace(Reason)) Console.Write("Appointment Reason cannot be null. Please enter a valid reason: ");
                else break;
            }
        }
        public override string ToString()
        {
            return $"Appointment ID: {Id} Patient Name: {PatientName}, Age: {PatientAge}, Date: {AppointmentDate}, Reason: {Reason}";
        }
        public int CompareTo(Appointment? other)
        {
            return this.Id.CompareTo(other?.Id);
        }
        public bool Equals(Appointment? other)
        {
            return this.Id == other?.Id;
        }
    }
}
