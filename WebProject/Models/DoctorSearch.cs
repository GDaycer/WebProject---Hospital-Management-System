using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebProject.Models;
using System.Web.Mvc;
namespace WebProject.Models
{
    public class DoctorSearch
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SelectedSpecialty { get; set; }

       
        public IEnumerable<SelectListItem> SpecialtyList { get; set; }

        
        public List<Doctor> Results { get; set; }
    }
}