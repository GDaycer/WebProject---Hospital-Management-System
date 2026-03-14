using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebProject.Models
{
    public class ImportantVisit
    {
        public int RecordId { get; set; }
        public DateTime VisitDate { get; set; }
        public string DoctorName { get; set; }
        public string Specialty { get; set; }
        public string Diagnosis { get; set; }
        public string Prescription { get; set; }
    }
}