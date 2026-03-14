using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebProject.Models
{
    public class PatientHomeView
    {
        public string FullName { get; set; }
        public int Age { get; set; }
        public string BloodGroup { get; set; }
        public int Height { get; set; }
        public int Weight { get; set; }

        public List<AppointmentViewModel> UpcomingAppointments { get; set; }
    }

    public class AppointmentViewModel
    {
        public string DoctorName { get; set; }
        public string Specialty { get; set; }
        public DateTime AppointmentDate { get; set; }
    }
}