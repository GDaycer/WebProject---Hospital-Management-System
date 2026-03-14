using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebProject.Filters;
using WebProject.Models;
namespace WebProject.Controllers
{

    public class PatientController : Controller
    {
        private ProjectDbContext DB = new ProjectDbContext();
        [AuthorizeRole("Patient")]
        public ActionResult Index()
        {
            return RedirectToAction("Home");
        }
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Register()
        {
            PatientSignUp model = new PatientSignUp();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult Register(PatientSignUp model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    Email = model.Email,
                    Password = System.Web.Helpers.Crypto.HashPassword(model.Password),
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Role = "Patient"
                };
                
                DB.Users.Add(user);
                DB.SaveChanges();

                var patient = new Patient
                {
                    UserId = user.UserId, // Use the ID from the user we just created
                    BirthDate = model.BirthDate,
                    Gender = model.Gender,
                    Address = model.Address,
                    PhoneNumber = model.PhoneNumber,
                    EmergencyContactName = model.EmergencyContactName,
                    EmergencyContactNumber = model.EmergencyContactNumber,
                    BloodType = model.BloodType,
                    Height = model.Height,
                    Weight = model.Weight
                };

                
                DB.Patients.Add(patient);
                DB.SaveChanges();

                return RedirectToAction("Login", "Account");
            }
            return View(model);
        }
        [AuthorizeRole("Patient")]
        public ActionResult Home()
        {

            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int sessionUserId = (int)Session["UserId"];

          
            var patientEntity = DB.Patients
                                  .Include("User") 
                                  .FirstOrDefault(p => p.UserId == sessionUserId);

            if (patientEntity == null)
            { 
                return RedirectToAction("Login", "Account");
            }

           
           
            var appointmentList = DB.Appointments
                                    .Include("Doctor")
                                    .Where(a => a.patientId == patientEntity.PatientId
                                                && a.startTime > DateTime.Now
                                                && a.status != "Cancelled")
                                    .OrderBy(a => a.startTime)
                                    .Take(3)
                                    .Select(a => new AppointmentViewModel
                                    {
                                        DoctorName = "Dr. " + a.Doctor.FirstName + " " + a.Doctor.LastName,
                                        Specialty = a.ReasonForVisit,
                                        AppointmentDate = a.startTime
                                    })
                                    .ToList();

            var model = new PatientHomeView
            {
                FullName = patientEntity.User.FirstName + " " + patientEntity.User.LastName,
                BloodGroup = patientEntity.BloodType,
                Height = patientEntity.Height ?? 0, //if null, show zero
                Weight = patientEntity.Weight ?? 0,
                UpcomingAppointments = appointmentList
            };

            return View(model);
        }
   

        [AuthorizeRole("Patient")]
        public ActionResult Appointments()
        {
            if (Session["UserId"] == null) return RedirectToAction("Login", "Account");
            int userId = (int)Session["UserId"];

            var patient = DB.Patients.Include(p => p.User).FirstOrDefault(p => p.UserId == userId);
            if (patient == null) return RedirectToAction("Login", "Account");

            var list = DB.Appointments
                         .Include(a => a.Doctor)
                         .Where(a => a.patientId == patient.PatientId)
                         .OrderByDescending(a => a.startTime)
                         .ToList();

            var nextAppt = list.Where(a => a.startTime > DateTime.Now && a.status != "Cancelled")
                               .OrderBy(a => a.startTime)
                               .FirstOrDefault();

            var model = new PatientAppointment
            {
                FullName = patient.User.FirstName + " " + patient.User.LastName,

                TotalCompleted = list.Count(a => a.startTime < DateTime.Now && a.status != "Cancelled"),
                TotalCancelled = list.Count(a => a.status == "Cancelled"),

                NextAppointmentDate = nextAppt?.startTime,

                AppointmentList = list
            };

            return View(model);
        }

        [AuthorizeRole("Patient")]


        public ActionResult Book(int? doctorId)
        {
            var model = new BookAppointment 
            {
                Date = DateTime.Today.AddDays(1),

                DoctorId = doctorId ?? 0,
                DoctorsList = DB.Doctors.Include(d => d.User).ToList().Select(d => new SelectListItem
                {
                    Value = d.DoctorId.ToString(),
                    Text = $"Dr. {d.User.FirstName} {d.User.LastName} ({d.Specialty})"
                })
            };

            
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Book(BookAppointment model)
        {
            // Reload the list in case of error
            model.DoctorsList = DB.Doctors.Include(d => d.User).ToList().Select(d => new SelectListItem
            {
                Value = d.DoctorId.ToString(),
                Text = $"Dr. {d.User.FirstName} {d.User.LastName} ({d.Specialty})"
            });

            if (ModelState.IsValid)
            {
                if (Session["UserId"] == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                DateTime appointmentDateTime = model.Date.Add(model.Time);//Combine date and time
                DateTime appointmentEnd = appointmentDateTime.AddMinutes(30);


                if (appointmentDateTime < DateTime.Now) //Cannot book in the past
                {
                    ModelState.AddModelError("", "You cannot book an appointment in the past.");
                    return View(model);
                }


                var doctor = DB.Doctors.Find(model.DoctorId); //check doctor shift
                if (doctor != null && doctor.ShiftStart != null && doctor.ShiftEnd != null)
                {
                    TimeSpan endTimeSpan = model.Time.Add(TimeSpan.FromMinutes(30)); //make sure 30 minutes is enough for doctor to finish his work

                    if (model.Time < doctor.ShiftStart ||endTimeSpan > doctor.ShiftEnd)
                    {
                        ModelState.AddModelError("", $"Dr. {doctor.User.LastName} is not working at this time. Their shift is {doctor.ShiftStart} - {doctor.ShiftEnd}.");
                        return View(model);
                    }
                }


                bool isTaken = DB.Appointments.Any(a =>  //check for overlapping appointments
                    a.doctorId == doctor.UserId &&
                    a.status != "Cancelled" &&
                    a.startTime < appointmentEnd &&
                    a.endTime > appointmentDateTime
                );

                if (isTaken)
                {
                    ModelState.AddModelError("", "This time slot is already taken. Please choose another time.");
                    return View(model);
                }


                int patientUserId = (int)Session["UserId"]; //save to database
                var patient = DB.Patients.FirstOrDefault(p => p.UserId == patientUserId);


                var newAppointment = new Appointment
                {
                    patientId = patient.PatientId,
                    doctorId = doctor.UserId,
                    startTime = appointmentDateTime,
                    endTime = appointmentEnd,
                    ReasonForVisit = model.Reason,
                    status = "Scheduled",
                    CreatedAt = DateTime.Now
                };

                DB.Appointments.Add(newAppointment);
                DB.SaveChanges();

                return RedirectToAction("Home");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole("Patient")]
        public ActionResult CancelAppointment(int id)
        {


            var appointment = DB.Appointments.Find(id);

            int userId = (int)Session["UserId"];
            var patient = DB.Patients.FirstOrDefault(p => p.UserId == userId);

            if (appointment != null && appointment.patientId == patient.PatientId)
            {
                if (appointment.startTime > DateTime.Now)
                {
                    appointment.status = "Cancelled";
                    DB.SaveChanges();
                }
            }

            return RedirectToAction("Appointments");
        }
        [AuthorizeRole("Patient")]
        public ActionResult Records()
        {
            int userId = (int)Session["UserId"];
            var patient = DB.Patients.Include(p => p.User).FirstOrDefault(p => p.UserId == userId);
            if (patient == null)
                return RedirectToAction("Login", "Account");

            ViewBag.FullName = patient.User.FirstName + " " + patient.User.LastName;
            ViewBag.BloodGroup = patient.BloodType;
            ViewBag.Height = patient.Height ?? 0;
            ViewBag.Weight = patient.Weight ?? 0;


            var records = DB.MedicalRecords
                            .Include(r => r.Doctor.User).Where(r => r.PatientId == patient.PatientId)
                                .OrderByDescending(r => r.CreatedAt).ToList();

            return View(records);
        }

        [HttpGet]
        [AuthorizeRole("Patient")]
        public ActionResult Profile()
        {
            int userId = (int)Session["UserId"];

            var patient = DB.Patients.Include(p => p.User).FirstOrDefault(p => p.UserId == userId); //get user+patient info

            if (patient == null) return RedirectToAction("Login", "Account");

            return View(patient);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole("Patient")]
        public ActionResult UpdateProfile(Patient model)
        {
            int userId = (int)Session["UserId"];
            var patientInDb = DB.Patients.Include(p => p.User).FirstOrDefault(p => p.UserId == userId);

            if (patientInDb != null)
            {
                patientInDb.PhoneNumber = model.PhoneNumber;
                patientInDb.Address = model.Address;
                patientInDb.EmergencyContactName = model.EmergencyContactName;
                patientInDb.EmergencyContactNumber = model.EmergencyContactNumber;
                patientInDb.Height = model.Height;
                patientInDb.Weight = model.Weight;

                DB.SaveChanges();
                TempData["SuccessMessage"] = "Profile updated successfully.";

                return RedirectToAction("Profile");
            }

            return View(model);
        }
        [AuthorizeRole("Patient")]
        public ActionResult GetImportantVisits()
        {
            int userId = (int)Session["UserId"];
            var patient = DB.Patients.FirstOrDefault(p => p.UserId == userId);

            if (patient == null) return Content("Patient not found");


            var lastRecord = DB.MedicalRecords //get the latest record
                               .Where(r => r.PatientId == patient.PatientId)
                               .OrderByDescending(r => r.CreatedAt)
                               .Select(r => new
                               {
                                   Diagnosis = r.Diagnosis,
                                   Prescription = r.Prescriptions,
                                   DoctorName = r.Doctor.User.LastName
                               })
                               .FirstOrDefault();


            if (lastRecord != null)
            {
                ViewBag.Diagnosis = lastRecord.Diagnosis;
                ViewBag.Prescription = lastRecord.Prescription;
                ViewBag.FooterNote = "Prescribed by Dr. " + lastRecord.DoctorName;
            }
            else
            {
                ViewBag.Diagnosis = "--";
                ViewBag.Prescription = "--";
                ViewBag.FooterNote = "No records found.";
            }

            return PartialView("_ImportantVisit");
        }

        [AuthorizeRole("Patient")]
        public ActionResult GetDefaultProfileButton()
        {
            return PartialView("_ProfileButton");
        }

    }
}