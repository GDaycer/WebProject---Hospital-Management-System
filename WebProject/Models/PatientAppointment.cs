using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebProject.Models
{
    public class PatientAppointment
    {
        public string FullName { get; set; }

        public int TotalCompleted { get; set; }
        public int TotalCancelled { get; set; }

        public DateTime? NextAppointmentDate { get; set; }

        public List<Appointment> AppointmentList { get; set; }
    }
}