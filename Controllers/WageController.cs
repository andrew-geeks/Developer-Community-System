using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.EnterpriseServices;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using DCS_new.Models;

namespace DCS_new.Controllers
{
    public class WageController : Controller
    {
        EmployeeEntities emp = new EmployeeEntities(); //employee model
        SpacesEntities2 Space = new SpacesEntities2(); //space and business model
        // GET: Wage
        public ActionResult Index()
        {
            if ((string)Session["actype"] == "business")
            {
                var bmail = Session["email"];
                Business singleData = Space.Businesses.Where(x => x.email.Equals(bmail.ToString())).SingleOrDefault();
                var b_id = singleData.b_id;
                var count = emp.employees.Where(x => x.b_id == b_id).Count();
                ViewData["ecount"] = count; //employee-count

                var empwage = emp.employees.Where(x => x.b_id == b_id).ToList();
                ViewData["empwage"] = empwage;

                //wage - sum
                var wage_sum = emp.employees.Where(x => x.b_id == b_id).Sum(i => i.wage);
                ViewData["wage_sum"] = wage_sum;

                return View(empwage);
            }
            else
            {
                return Content("<script language='javascript' type='text/javascript'>alert('Invalid URL!');</script>");
            }
            
        }

        public ActionResult setWages()
        {
            if ((string)Session["actype"] == "business")
            {
                var bmail = Session["email"];
                Business singleData = Space.Businesses.Where(x => x.email.Equals(bmail.ToString())).SingleOrDefault();
                var b_id = singleData.b_id;
                var empwage = emp.employees.Where(x => x.b_id == b_id).ToList();
                ViewData["empwage"] = empwage;

                
                return View();
            }
            else
            {
                return Content("<script language='javascript' type='text/javascript'>alert('Invalid URL!');</script>");
                
            }
        }

        [HttpPost]
        public ActionResult setWages(employee employee)
        {
            var eid = employee.e_id;
            var bid = Convert.ToInt32(Session["b_id"]);
            var new_wage = employee.wage;
            employee singleData = emp.employees.Where(x => x.e_id == eid && x.b_id == bid).SingleOrDefault();
            try
            {
                singleData.wage = new_wage;
                emp.Entry(singleData).State = System.Data.Entity.EntityState.Modified;
                emp.SaveChanges();
                if(singleData.empl_type == "Freelancer")
                {
                    freelancer fdata = Space.freelancers.Where(x => x.email == singleData.email).SingleOrDefault();
                    fdata.wage = new_wage;
                    Space.Entry(fdata).State = System.Data.Entity.EntityState.Modified;
                    Space.SaveChanges();
                }
                return Content("<script language='javascript' type='text/javascript'>alert('Wage Updated!');</script>");
            }
            catch
            {
                return Content("<script language='javascript' type='text/javascript'>alert('Error!');</script>");
            }
            


        }

        public ActionResult pay() //transaction-module
        {
            if ((string)Session["actype"] == "business")
            {
                var bmail = Session["email"];
                Business singleData = Space.Businesses.Where(x => x.email.Equals(bmail.ToString())).SingleOrDefault();
                var b_id = singleData.b_id; //b_id
                Business b_name = Space.Businesses.Where(x => x.b_id == b_id).SingleOrDefault();
                var buss_name = b_name.b_name; //business_name
                ViewData["business_name"] = buss_name;
                //wage - sum
                var wage_sum = emp.employees.Where(x => x.b_id == b_id).Sum(i => i.wage);
                ViewData["wage_sum"] = wage_sum;
                DateTime today = DateTime.Today;
                ViewData["t_date"] = DateTime.Now.ToString("d/M/yyyy");

                //check-if-paid-this-month
                var fidate = "01/" + Convert.ToString(today.Month) + "/" + Convert.ToString(today.Year);
                DateTime fdate = DateTime.ParseExact(fidate, "d/M/yyyy", null);
                ViewData["month"] = fdate;
                var singledata = Space.wages.Where(x => x.date >= fdate && x.b_id == b_id).FirstOrDefault();
                if (singledata != null)
                {
                    ViewData["this_month_wage_sum"] = 0;
                }
                else
                {
                    ViewData["this_month_wage_sum"] = wage_sum;
                }
                return View();
            }
            else
            {
                return Content("<script language='javascript' type='text/javascript'>alert('Error!');</script>");
            }
            
        }

        [HttpPost]
        public ActionResult pay(wage wage)
        {
            var bmail = Session["email"];
            Business singleData = Space.Businesses.Where(x => x.email.Equals(bmail.ToString())).SingleOrDefault();
            var b_id = singleData.b_id; //b_id

            var empdata = emp.employees.Where(x => x.b_id == b_id).ToList();
            
            foreach (var data in empdata)
            {
                wage newwg = new wage();
                newwg.e_id = data.e_id;
                newwg.b_id = (int)data.b_id;
                newwg.amount = (int)data.wage;
                newwg.date = DateTime.Today;

                Space.wages.Add(newwg);
                Space.SaveChanges();


            }
            return RedirectToAction("pay", "Wage");
        }
    }
}