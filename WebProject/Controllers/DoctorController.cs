using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebProject.Models;
using WebProject.Filters;


namespace WebProject.Controllers
{
    public class DoctorController : Controller
    {

        private ProjectDbContext db = new ProjectDbContext();
        [AuthorizeRole("Admin")]
        public ActionResult Create()
        {
            return View(new DoctorSignUp());
        }
        [AuthorizeRole("Admin")]
        public ActionResult List()
        {
            var doctors = db.Doctors.Include(d => d.User);
            return View(doctors.ToList());
        }

        // POST: Doctors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole("Admin")]
        public ActionResult Create(DoctorSignUp model)
        {
            if (ModelState.IsValid)
            {

                var user = new User //First Create the User
                {
                    Email = model.Email,
                    Password = System.Web.Helpers.Crypto.HashPassword(model.Password),
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Role = "Doctor"
                };

                db.Users.Add(user);
                db.SaveChanges(); // Generates the UserId


                var doctor = new Doctor //Create doctor with userId
                {
                    UserId = user.UserId,
                    Specialty = model.Specialty,
                    LicenseNumber = model.LicenseNumber,
                    Bio = model.Bio,
                    ShiftStart = model.ShiftStart,
                    ShiftEnd = model.ShiftEnd
                };

                db.Doctors.Add(doctor);
                db.SaveChanges();

                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }
        [AuthorizeRole("Admin")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Doctor doctor = db.Doctors.Include(d => d.User).FirstOrDefault(d => d.DoctorId == id);
            if (doctor == null)
            {
                return HttpNotFound();
            }
            return View(doctor);
        }
        [AuthorizeRole("Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Doctor doctor = db.Doctors.Include(d => d.User).FirstOrDefault(d => d.DoctorId == id);
            if (doctor == null)
            {
                return HttpNotFound();
            }
            return View(doctor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole("Admin")]
        public ActionResult Edit(Doctor doctor)
        {
            if (ModelState.IsValid)
            {
                var doctorInDb = db.Doctors.Include(d => d.User).FirstOrDefault(d => d.DoctorId == doctor.DoctorId);

                if (doctorInDb != null)
                {
                    doctorInDb.Specialty = doctor.Specialty;
                    doctorInDb.LicenseNumber = doctor.LicenseNumber;
                    doctorInDb.Bio = doctor.Bio;
                    doctorInDb.ShiftStart = doctor.ShiftStart;
                    doctorInDb.ShiftEnd = doctor.ShiftEnd;

                    doctorInDb.User.FirstName = doctor.User.FirstName;
                    doctorInDb.User.LastName = doctor.User.LastName;
                    doctorInDb.User.Email = doctor.User.Email;

                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(doctor);
        }
        [AuthorizeRole("Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Doctor doctor = db.Doctors.Include(d => d.User).FirstOrDefault(d => d.DoctorId == id);
            if (doctor == null)
            {
                return HttpNotFound();
            }
            return View(doctor);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuthorizeRole("Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            Doctor doctor = db.Doctors.Find(id);

            User user = db.Users.Find(doctor.UserId);

            db.Doctors.Remove(doctor);

            if (user != null)
            {
                db.Users.Remove(user);
            }

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Index()
        {
            var model = new DoctorSearch
            {
                SpecialtyList = db.Doctors
                                  .Select(d => d.Specialty)
                                  .Distinct()
                                  .ToList()
                                  .Select(s => new SelectListItem
                                  {
                                      Value = s,
                                      Text = s
                                  }),
                Results = db.Doctors.Include(d => d.User).ToList()
            };
            return View(model);
        }

        public ActionResult Search(DoctorSearch model)
        {
            var query = db.Doctors.Include(d => d.User).AsQueryable();

            
            if (!string.IsNullOrEmpty(model.FirstName)) //filter first name
            {
                query = query.Where(d => d.User.FirstName.Contains(model.FirstName));
            }

            
            if (!string.IsNullOrEmpty(model.LastName)) //filter last name
            {
                query = query.Where(d => d.User.LastName.Contains(model.LastName));
            }

            
            if (!string.IsNullOrEmpty(model.SelectedSpecialty)) //Filter specialty
            {
                query = query.Where(d => d.Specialty == model.SelectedSpecialty);
            }


            return PartialView("_DoctorList", query.ToList());
        }


        [AuthorizeRole("Admin","Doctor")]
        public ActionResult HomeScreen()
        {
            int userId = (int)Session["UserId"];
            DateTime today = DateTime.Today;

            var doctor = db.Doctors.Include(d => d.User).FirstOrDefault(d => d.UserId == userId);
            if (doctor == null) return RedirectToAction("List");

            ViewBag.DoctorName = "Dr. " + doctor.User.FirstName + " " + doctor.User.LastName;
            ViewBag.Specialty = doctor.Specialty;


            var appointments = db.Appointments.Include(a => a.Patient.User)
                                 .Where(a => a.doctorId == userId
                                             && DbFunctions.TruncateTime(a.startTime) == today
                                             && a.status != "Cancelled")
                                 .OrderBy(a => a.startTime).ToList();

 
            return View(appointments);
        }


        [AuthorizeRole("Doctor")]
        public ActionResult Schedule()
        {
            int userId = (int)Session["UserId"];

            var appointments = db.Appointments.Include(a => a.Patient.User).Where(a => a.doctorId == userId)
                              .OrderByDescending(a => a.startTime).ToList();
            return View(appointments);
        }


        [AuthorizeRole("Doctor")]
        public ActionResult MyPatients()
        {
            int userId = (int)Session["UserId"];


            var patients = db.Appointments.Where(a => a.doctorId == userId).Select(a => a.Patient)
                        .Distinct().Include(p => p.User).ToList();

            return View(patients);
        }

        [HttpGet]
        [AuthorizeRole("Doctor")]
        public ActionResult DoctorProfile()
        {
            int userId = (int)Session["UserId"];
            var doctor = db.Doctors.Include(d => d.User).FirstOrDefault(d => d.UserId == userId);
            if (doctor == null) return RedirectToAction("HomeScreen"); //safety check

            return View(doctor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole("Doctor")]
        public ActionResult UpdateBio(Doctor model)
        {
            if (ModelState.IsValid)
            {
                int userId = (int)Session["UserId"];
                var doctorInDb = db.Doctors.FirstOrDefault(d => d.UserId == userId);
                if (doctorInDb != null)
                {
                    doctorInDb.Bio = model.Bio;
                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Bio updated successfully.";
                }
            }
            return RedirectToAction("DoctorProfile");
        }


        [HttpGet]
        [AuthorizeRole("Doctor")]
        public ActionResult CreateRecord()
        {
            int userId = (int)Session["UserId"];

  
            var patients = db.Appointments.Where(a => a.doctorId == userId).Select(a => a.Patient)
                                          .Distinct().Include(p => p.User).ToList();


            ViewBag.PatientId = new SelectList(patients.Select(p => new { 
                    PatientId = p.PatientId,
                    FullName = p.User.FirstName + " " + p.User.LastName
            }), "PatientId", "FullName");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeRole("Doctor")]
        public ActionResult CreateRecord(MedicalRecord model)
        {
            int userId = (int)Session["UserId"];
            var doctor = db.Doctors.FirstOrDefault(d => d.UserId == userId);


            model.CreatedAt = DateTime.Now;
            model.DoctorId = doctor.DoctorId;


            var latestAppointment = db.Appointments
                                      .Where(a => a.patientId == model.PatientId && a.doctorId == userId)
                                      .OrderByDescending(a => a.startTime)
                                      .FirstOrDefault();

            if (latestAppointment != null)
            {
                model.AppointmentId = latestAppointment.appointmentId;
            }


            if (ModelState.IsValid)
            {
                db.MedicalRecords.Add(model);
                db.SaveChanges();
                return RedirectToAction("MyPatients");
            }


            var patients = db.Appointments.Where(a => a.doctorId == userId)   //reload if validation is failed
                                          .Select(a => a.Patient).Distinct().Include(p => p.User).ToList();
            ViewBag.PatientId = new SelectList(patients.Select(p => new {
                PatientId = p.PatientId,
                FullName = p.User.FirstName + " " + p.User.LastName
            }), "PatientId", "FullName");

            return View(model);
        }

        [AuthorizeRole("Doctor")]
        public ActionResult PatientHistory(int id) 
        {

            var records = db.MedicalRecords
                            .Include(r => r.Doctor.User) 
                            .Where(r => r.PatientId == id)
                            .OrderByDescending(r => r.CreatedAt) 
                            .ToList();

            var patient = db.Patients
                            .Include(p => p.User)
                            .FirstOrDefault(p => p.PatientId == id);



            ViewBag.PatientName = patient.User.FirstName + " " + patient.User.LastName;

            return View(records);
        }
    }
}