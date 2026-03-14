using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebProject.Models;
using WebProject.Filters;
namespace WebProject.Controllers
{
    public class HomeController : Controller
    {
        private ProjectDbContext DB = new ProjectDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AboutUs()
        {
            return View();
        }

        public ActionResult ContactUs()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult SignUp(PatientSignUp model)
        {
            if (ModelState.IsValid)
            {
                if (DB.Users.Any(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Email already exists");
                    return View(model);
                }

                //create user
                User user = new User
                {
                    Email = model.Email,
                    Password = model.Password,
                    Role = "Patient",
                    FirstName = model.FirstName,
                    LastName = model.LastName
                };

                DB.Users.Add(user);
                DB.SaveChanges(); //Save to get UserId

                //create patient
                Patient patient = new Patient
                {
                    UserId = user.UserId,
                    BirthDate = model.BirthDate,
                    Gender = model.Gender,
                    Address = model.Address,
                    PhoneNumber = model.PhoneNumber,
                    EmergencyContactName = model.EmergencyContactName,
                    EmergencyContactNumber = model.EmergencyContactNumber,
                    BloodType = model.BloodType
                };
                DB.Patients.Add(patient);
                DB.SaveChanges();
                return RedirectToAction("Login");
            }
            return View(model);
        }
    }
}


