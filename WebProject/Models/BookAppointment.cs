using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace WebProject.Models
{
    public class BookAppointment
    {
        [Required(ErrorMessage = "Please select a doctor")]
        [Display(Name = "Select Doctor")]
        public int DoctorId { get; set; }


        [Required(ErrorMessage = "Please select a date")]
        [DataType(DataType.Date)]
        [Display(Name = "Appointment Date")]
        public DateTime Date { get; set; }


        [Required(ErrorMessage = "Please select a time")]
        [DataType(DataType.Time)]
        [Display(Name = "Preferred Time")]
        public TimeSpan Time { get; set; }


        [Required(ErrorMessage = "Please enter a reason")]
        [StringLength(500)]
        [Display(Name = "Reason for Visit")]
        public string Reason { get; set; }


        public IEnumerable<SelectListItem> DoctorsList { get; set; }
    }
}