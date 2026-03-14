using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebProject.Models
{
    public class Appointment
    {
        [Key]
        public int appointmentId { get; set; }

        [ForeignKey("Patient")]
        public int patientId { get; set; }
        public virtual Patient Patient { get; set; }

        [ForeignKey("Doctor")]
        public int doctorId { get; set; }
        public virtual User Doctor { get; set; }
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
        public string status { get; set; }
        [Column(TypeName = "ntext")] // Allows for a long text block
        public string ReasonForVisit { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}