using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebProject.Models
{
    public class MedicalRecord
    {
        [Key]
        public int RecordId { get; set; }

        [ForeignKey("Patient")]
        public int PatientId { get; set; }
        public virtual Patient Patient { get; set; }

        [ForeignKey("Appointment")]
        public int AppointmentId { get; set; }
        public virtual Appointment Appointment { get; set; }

        [ForeignKey("Doctor")]
        public int DoctorId { get; set; }
        public virtual Doctor Doctor { get; set; }
        [Required(ErrorMessage ="Please enter a diagnosis")]
        public string Diagnosis { get; set; }

        [Column(TypeName = "ntext")] // Allows for a long text block
        [Required(ErrorMessage = "Please write your observations")]
        public string Notes { get; set; }
        public string Prescriptions { get; set; }
        public DateTime CreatedAt { get; set; }



    }
}