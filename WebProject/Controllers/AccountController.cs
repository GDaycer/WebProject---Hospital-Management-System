using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebProject.Models;

namespace WebProject.Controllers
{
    public class AccountController : Controller
    {
        private ProjectDbContext DB = new ProjectDbContext();
        // GET: /Account/Login
        [HttpGet]
        public ActionResult Login()
        {
            return View("~/Views/Home/Login.cshtml");
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginView model)
        {
            if (ModelState.IsValid)
            {
                
                
                var user = DB.Users.FirstOrDefault(u => u.Email == model.Email);    



                if (user != null && System.Web.Helpers.Crypto.VerifyHashedPassword(user.Password, model.Password))
                {
                    if(user.Role != "Admin")
                    {
                        if (model.Role != null && user.Role != model.Role)
                        {
                            ViewBag.ErrorMessage = "You selected the wrong role for this account.";
                            return View("~/Views/Home/Login.cshtml", model);
                        }
                    }

                    Session["UserId"] = user.UserId;
                    Session["UserRole"] = user.Role;

                    if (user.Role == "Patient")
                    {
                        return RedirectToAction("Home", "Patient");
                    }
                    else if (user.Role == "Doctor")
                    {
                        return RedirectToAction("HomeScreen", "Doctor");
                    }
                    else if (user.Role == "Admin")
                    {
                        return RedirectToAction("List", "Doctor");
                    }
                }

                ViewBag.ErrorMessage = "Invalid email or password.";
            }

            return View("~/Views/Home/Login.cshtml", model);
        }

        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public ActionResult AccessDenied()
        {
            return View();
        }

    }
}
