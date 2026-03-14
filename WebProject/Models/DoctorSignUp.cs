using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebProject.Models
{
    public class DoctorSignUp
    {
        // --- User Account Fields ---
        [Key]
        public int DoctorSignUpId { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        // --- Doctor Profile Fields ---
        [Required]
        public string Specialty { get; set; }

        [Required]
        [Display(Name = "License Number")]
        public string LicenseNumber { get; set; }

        [DataType(DataType.MultilineText)]
        public string Bio { get; set; }

        [Display(Name = "Shift Start Time")]
        [DataType(DataType.Time)]
        public TimeSpan? ShiftStart { get; set; }

        [Display(Name = "Shift End Time")]
        [DataType(DataType.Time)]
        public TimeSpan? ShiftEnd { get; set; }
    }
}