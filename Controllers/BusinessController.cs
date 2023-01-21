using DCS_new.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Xml.Linq;

namespace DCS_new.Controllers
{
    public class BusinessController : Controller
    {
        SpacesEntities2 Spaces = new SpacesEntities2(); //space model & business
        EmployeeEntities emp = new EmployeeEntities();

       
        public ActionResult Index()
        {
            if ((string)Session["actype"] == "business")
            {
                var bmail = Session["email"];
                
                Business singleData = Spaces.Businesses.Where(x => x.email.Equals(bmail.ToString())).SingleOrDefault();
                var count = emp.employees.Where(x => x.b_id == singleData.b_id).Count();
                ViewData["ecount"] = count;
                ViewData["bname"] = singleData.b_name;
                ViewData["bmail"] = singleData.email;
                ViewData["bid"] = singleData.b_id;
                var bid = singleData.b_id;
 
                var sp = Spaces.spaces.Where(x => x.b_id == bid).ToList();

                ViewData["spacedata"] = sp;
                
                return View(sp);

                
                

            }
            else if ((string)Session["actype"] == "employee")
            {
                return RedirectToAction("Index", "Account");
            }
            else
            {
                return RedirectToAction("bLogin", "Home");
            }
            
        }

        public ActionResult editBProfile()
        {
            if ((string)Session["actype"]=="business")
            {
                var email = Session["email"];
                ViewData["email"] = email;
                return View();
                
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
            
        }

        [HttpPost]
        public ActionResult editBProfile(Business business)
        {
            var email = Session["email"];
            var bdata = Spaces.Businesses.Where(x => x.email == (string)email).SingleOrDefault();
            bdata.b_name = business.b_name;
            Spaces.Entry(bdata).State = System.Data.Entity.EntityState.Modified;
            Spaces.SaveChanges();
            return RedirectToAction("index", "business");
        }

        public ActionResult changeBPassword()
        {
            if (Session["actype"] == "business")
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
           
        }

        [HttpPost]
        public ActionResult changeBPassword(Business business)
        {
            var email = Session["email"];
            Business bdata = Spaces.Businesses.Where(x => x.email == (string)email).SingleOrDefault();
            bdata.password = business.password;
            Spaces.Entry(bdata).State = System.Data.Entity.EntityState.Modified;
            Spaces.SaveChanges();
            return RedirectToAction("index", "business");
            
        }
        public ActionResult createSpace()
        {
            if ((string)Session["actype"] == "business")
            {
                return View();

            }
            else
            {
                return RedirectToAction("bLogin", "Home");
            }
                
        }

        [HttpPost]
        public ActionResult createSpace(space space)
        {
            var check_head = Spaces.spaces.Where(x => x.head_e_id == space.head_e_id).FirstOrDefault();
            if (check_head == null) //check-no-head-exists
            {
                try
                {
                    var bid = (int)Session["b_id"] + 0;
                    space singledata = new space();
                    singledata.s_name = space.s_name;
                    singledata.s_status = space.s_status;
                    singledata.b_id = bid;
                    singledata.head_e_id = space.head_e_id;
                    Spaces.spaces.Add(singledata);
                    Spaces.SaveChanges();

                    var sp_data = Spaces.spaces.Where(s => s.s_name.Equals(space.s_name)).FirstOrDefault();
                    var head_e_id = sp_data.head_e_id; //retreving head_e_id
                    var empsingle = emp.employees.Where(e => e.e_id == head_e_id).FirstOrDefault();
                    empsingle.s_id = sp_data.s_id;
                    emp.Entry(empsingle).State = System.Data.Entity.EntityState.Modified;
                    emp.SaveChanges();

                    return RedirectToAction("Index", "Business");
                }
                catch
                {
                    return Content("<script language='javascript' type='text/javascript'>alert('Error!');</script>");
                }
            }
            else
            {
                return Content("<script language='javascript' type='text/javascript'>alert('Error!');</script>");
            }
            
            
           
            
            
        }

        [Route("/business/employeedata/{bid}")]
        public ActionResult employeedata(int bid)
        {
            if ((string)Session["actype"] == "business")
            {
                Business bdata = Spaces.Businesses.Where(x=>x.b_id == bid).FirstOrDefault();
                if(bdata != null)
                {
                    var empdata = emp.employees.Where(x => x.b_id == bid).ToList();
                    ViewData["empdata"] = empdata;
                    return View(empdata);
                }
                else
                {
                    return Content("<script language='javascript' type='text/javascript'>alert('Invalid ID!');</script>");
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }


        [Route("/business/freelancerdata/{bid}")]
        public ActionResult freelancerdata(int bid)
        {
            if ((string)Session["actype"] == "business")
            {
                Business bdata = Spaces.Businesses.Where(x => x.b_id == bid).FirstOrDefault();
                if (bdata != null)
                {
                    var fdata = Spaces.freelancers.Where(x => x.b_id == bid).ToList();
                    ViewData["fdata"] = fdata;
                    return View(fdata);
                }
                else
                {
                    return Content("<script language='javascript' type='text/javascript'>alert('Invalid ID!');</script>");
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }



    }
}