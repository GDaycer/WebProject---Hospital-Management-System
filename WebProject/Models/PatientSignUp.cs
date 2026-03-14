using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebProject.Models
{
    public class PatientSignUp
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        [Required]
        public string ConfirmPassword { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        // Patient fields
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        public string Gender { get; set; }
        public string Address { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        public string EmergencyContactName { get; set; }

        [Phone]
        public string EmergencyContactNumber { get; set; }

        public string BloodType { get; set; }

        [Range(0, 400, ErrorMessage = "Please enter a valid height in cm")]
        public int? Height { get; set; }

        [Range(0, 500, ErrorMessage = "Please enter a valid weight in kg")]
        public int? Weight { get; set; }
    }
}