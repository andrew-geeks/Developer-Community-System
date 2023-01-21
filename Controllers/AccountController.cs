using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Xml.Linq;
using DCS_new.Models;

namespace DCS_new.Controllers
{
    public class AccountController : Controller
    {
        EmployeeEntities emp = new EmployeeEntities();
        SpacesEntities2 Spaces = new SpacesEntities2();  //space model & business
        // GET: Account
        public ActionResult Index() //employee acc. page
        {
            var Email = Session["email"];
            if (Session["email"] != null)
            {
                employee singleData = emp.employees.Where(x => x.email.Equals(Email.ToString())).SingleOrDefault();
                ViewData["name"] = singleData.name;
                ViewData["email"] = singleData.email;
                var bid = singleData.b_id; //business id
                if (singleData.s_id != null)
                {
                    var s_data = Spaces.spaces.Where(x => x.s_id == singleData.s_id).SingleOrDefault();
                    ViewData["url_sid"] = s_data.s_id;
                    ViewData["url_sp_name"] = s_data.s_name;

                }
                else
                {
                    ViewData["sp"] = "False";
                }
                Business bdata = Spaces.Businesses.Where(x => x.b_id == bid).SingleOrDefault();
                if (bdata != null)
                {
                    ViewData["admn"] = bdata.b_name;
                }
                else
                {
                    ViewData["admn"] = "";
                }
                
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
                
        }

        public ActionResult editProfile()
        {
            if (Session["email"] != null)
            {
                var Email = Session["email"];
                employee singleData = emp.employees.Where(x => x.email.Equals(Email.ToString())).SingleOrDefault();
                employee employee = new employee
                {
                    name = singleData.name,
                    b_id = singleData.b_id,
                    designation = singleData.designation
                }; //exception will be thrown if non-exisisting bid is given - please do needful!
                ViewBag.Message = employee;
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpPost]
        public ActionResult editProfile(employee employee)
        {
            if (Session["email"] != null)
            {
                var Email = Session["email"];
                employee singleData = emp.employees.Where(x => x.email.Equals(Email.ToString())).SingleOrDefault();
                singleData.name = employee.name;
                singleData.b_id = employee.b_id;
                singleData.designation = employee.designation;
                emp.Entry(singleData).State = System.Data.Entity.EntityState.Modified;
                emp.SaveChanges();

                if(singleData.empl_type == "Freelancer")
                {
                    freelancer fdata = Spaces.freelancers.Where(x => x.email.Equals(Email.ToString())).SingleOrDefault();
                    fdata.name = employee.name;
                    fdata.b_id = employee.b_id;
                    fdata.designation = employee.designation;
                    Spaces.Entry(fdata).State = System.Data.Entity.EntityState.Modified;
                    Spaces.SaveChanges();
                }
                return RedirectToAction("Index", "Account");


            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }



        public ActionResult changePassword()
        {
            if (Session["email"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpPost]
        public ActionResult changePassword(employee employee)
        {
            var email = Session["email"];
            var edata = emp.employees.Where(x => x.email == (string)email).SingleOrDefault();
            edata.password = employee.password;
            emp.Entry(edata).State = System.Data.Entity.EntityState.Modified;
            emp.SaveChanges();
            return RedirectToAction("index","account");
        }
    }
}