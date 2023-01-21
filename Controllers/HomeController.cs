using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DCS_new.Models;

namespace DCS_new.Controllers
{
    public class HomeController : Controller
    {
        SpacesEntities2 Spaces = new SpacesEntities2();  //space model & business
        EmployeeEntities emp = new EmployeeEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult bsignup() //business signup
        {
            if ((string)Session["actype"] == "business" && (string)Session["email"]!=null)
            {
                return RedirectToAction("Index", "Business");
            }
            else
            {
                return View();
            }
            
        }

        [HttpPost]

        public ActionResult bsignup(Business business) //business signup-post
        {
            if(Spaces.Businesses.Any(x => x.email == business.email))
            {
                ViewBag.Notification = "Emal already in use!";
                return View();
            }
            else
            {
                Business dbbusiness = new Business();
                dbbusiness.email = business.email;
                dbbusiness.b_name = business.b_name; //business name
                dbbusiness.password = business.password;

                Spaces.Businesses.Add(dbbusiness);
                Spaces.SaveChanges();

                Session["email"] = business.email.ToString();
                Session["b_id"] = business.b_id.ToString();
                Session["actype"] = "business";
                return RedirectToAction("Index", "Business"); 


            }
        }



        public ActionResult SignUp()
        {
            return View();
        }

        [HttpPost]

        public ActionResult SignUp(employee employee)
        {
            if(emp.employees.Any(x => x.email == employee.email))
            {
                ViewBag.Notification = "Email already in use!";
                return View();
            }
            else
            {
                employee dbempl = new employee(); //db employee object
                dbempl.name = employee.name;
                dbempl.email = employee.email;
                dbempl.password = employee.password;
                dbempl.empl_type = employee.empl_type;
                dbempl.status = "Online";


                emp.employees.Add(dbempl);
                emp.SaveChanges();
                //add to freelancer table if empl_type is freelancer!
                if(employee.empl_type == "Freelancer")
                {
                    freelancer dbfr = new freelancer();
                    dbfr.name = employee.name;
                    dbfr.email = employee.email;

                    Spaces.freelancers.Add(dbfr);
                    Spaces.SaveChanges();
                }
                Session["email"] = employee.email.ToString();
                Session["actype"] = "employee";
                return RedirectToAction("Index", "Home");
            }
            
        }

        [HttpGet]
        public ActionResult bLogin()
        {
            if ((string)Session["email"] != null && (string)Session["actype"] == "business")
            {
                return RedirectToAction("Index", "Business");
            }
            else if (Session["email"] != null && (string)Session["actype"] == "employee")
            {
                return RedirectToAction("Index", "Account");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult bLogin(Business business)
        {
            //querying
            var checkBlogin = Spaces.Businesses.Where(x => x.email.Equals(business.email) && x.password.Equals(business.password)).FirstOrDefault();
            if (checkBlogin != null)
            {
                Session["email"] = business.email;
                Session["actype"] = "business";
                var bmail = Session["email"];
                Business singleData = Spaces.Businesses.Where(x => x.email.Equals(bmail.ToString())).SingleOrDefault();
                Session["b_id"] = singleData.b_id;
                return RedirectToAction("Index", "Business"); //change
            }
            else
            {
                ViewBag.Notification = "Incorrect email/password!";
                return View();
            }

        }



        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(employee employee)
        {
            if (Session["email"] == null)
            {
                
                var checkLogin = emp.employees.Where(x => x.email.Equals(employee.email) && x.password.Equals(employee.password)).FirstOrDefault();
                if (checkLogin != null)
                {
                    Session["email"] = employee.email;
                    Session["actype"] = "employee";
                    var uempl = emp.employees.SingleOrDefault(e => e.email == employee.email);
                    uempl.status = "Online"; //setting status =>Online
                    emp.SaveChanges();
                    try
                    {
                        var head_id = Spaces.spaces.Where(e => e.head_e_id == uempl.e_id).SingleOrDefault();
                        if (head_id.head_e_id != null) //checking if the empl is a space head(grp admin)!
                        {
                            Session["sp_head"] = "True";
                        }
                    }
                    catch
                    {
                        Session["sp_head"] = "False";
                    }
                    return RedirectToAction("Index", "Home");

                }
                else
                {
                    ViewBag.Notification = "Incorrect email/password";
                }
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Account");
            }
            
        }

        public ActionResult about()
        {
            return View();
        }

        public ActionResult Logout()
        {
            if ((string)Session["actype"] == "employee")
            {
                var email = Convert.ToString(Session["email"]);
                var uempl = emp.employees.SingleOrDefault(e => e.email == email);
                uempl.status = "Offline";
                emp.SaveChanges();
            }
            
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}