using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace WebProject.Models
{
    public class Doctor
    {
        [Key]
        public int DoctorId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public string Specialty { get; set; }
        public string LicenseNumber { get; set; }

        [Column(TypeName = "ntext")] // Allows for a very long text block
        public string Bio { get; set; }

        public TimeSpan? ShiftStart { get; set; }

        public TimeSpan? ShiftEnd { get; set; }
    }
}