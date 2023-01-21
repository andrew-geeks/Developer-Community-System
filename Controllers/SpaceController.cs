using DCS_new.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Microsoft.AspNet.SignalR;
using Antlr.Runtime.Tree;
using System.EnterpriseServices;
using System.Data.Entity.Validation;
using System.Security.Cryptography;

namespace DCS_new.Controllers
{
    public class SpaceController : Controller
    {
        SpacesEntities2 Spaces = new SpacesEntities2();//space & business model
        EmployeeEntities emp = new EmployeeEntities();
        // GET: Space

        public SpaceController()
        {
        }

        public ActionResult Index(int sid)
        {
            return View();
        }

        /*https://localhost:44326/space/chat?sid=1 */
        [Route("space/chat/{sid}")]
        public ActionResult chat(int sid)
        {
            try
            {
                if ((string)Session["actype"] == "business" || (string)Session["actype"] == "employee")
                {
                    //verifiction required: if emp is a member of the accessed space!
                    space singledata = Spaces.spaces.Where(x => x.s_id.Equals(sid)).SingleOrDefault();
                    var empl_online = emp.employees.Where(x => x.s_id == sid && x.status == "Online").Count();
                    var empl_offline = emp.employees.Where(x => x.s_id == sid && x.status == "Offline").Count();
                    ViewData["sname"] = singledata.s_name;
                    ViewData["sid"] = singledata.s_id;
                    Session["curr_sid"] = singledata.s_id;
                    ViewData["sstatus"] = singledata.s_status;
                    ViewData["admnid"] = singledata.b_id;
                    ViewData["e_online"] = empl_online;
                    ViewData["e_offline"] = empl_offline;

                    var bmail = (string)Session["email"];
                    var e_id = 0;
                    //chatname
                    if ((string)Session["sp_head"] == "True" || (string)Session["actype"] == "employee"){ //employee/sp-head
                        employee empsingle = emp.employees.Where(x => x.email.Equals(bmail.ToString())).SingleOrDefault();
                        ViewData["chatname"] = empsingle.name;
                    }
                    else
                    {
                        Business bdata = Spaces.Businesses.Where(x => x.email == bmail).SingleOrDefault();
                        ViewData["chatname"] = bdata.b_name;
                    }

                    //tasks-get
                    if ((string)Session["sp_head"] == "True") //employee
                    {
                        var tdata = Spaces.tasks.Where(x => x.status == "Ongoing" && x.s_id == sid).ToList();
                        ViewData["tdata"] = tdata;
                        return View(tdata);
                    }
                    else if((string)Session["actype"] == "employee") //space heads
                    {
                       
                        employee empsingle = emp.employees.Where(x => x.email.Equals(bmail.ToString())).SingleOrDefault();
                        e_id = empsingle.e_id; //getting emp_id
                        
                        //getting tasks
                        var tdata = Spaces.tasks.Where(x => x.e_id == e_id && x.status == "Ongoing" && x.s_id == sid).ToList();
                        ViewData["tdata"] = tdata;
                        return View(tdata);
                    }
                    else //business
                    {
                        var tdata = Spaces.tasks.Where(x => x.status == "Ongoing" && x.s_id == sid).ToList();
                        ViewData["tdata"] = tdata;
                        return View(tdata);
                    }

                    
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }   
        }

        [Route("space/details/{sid}")]
        public ActionResult details(int sid)
        {
            space sp = Spaces.spaces.Where(x => x.s_id == sid).SingleOrDefault();
            if (sp != null)
            {
                var empdata = emp.employees.Where(x=>x.s_id == sid).ToList();
                ViewData["employeedata"] = empdata;
                return View(empdata);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        
        [Route("space/addmembers/{sid}")]

        public ActionResult addmembers(int sid)
        {
            if ((string)Session["sp_head"]=="True" || (string)Session["actype"] == "business")
            {
                var bmail = Session["email"];
                var b_id = 0;
                if ((string)Session["sp_head"] == "True") //employee
                {
                    employee empsingle = emp.employees.Where(x => x.email.Equals(bmail.ToString())).SingleOrDefault();
                    b_id = (int)empsingle.b_id;
                }
                else //business
                {
                    Business singleData = Spaces.Businesses.Where(x => x.email.Equals(bmail.ToString())).SingleOrDefault();
                    b_id = singleData.b_id;
                }

                var names = emp.employees.Where(c => c.b_id == b_id && c.s_id == null).Select(c => new { c.e_id,c.name}).ToList();
                ViewBag.namelist = names;

                return View();
            }
            else
            {
                return RedirectToAction("restricted", "Space");
            }
            
            
        }

        [Route("space/addmembers/{sid}")]
        [HttpPost]
        public ActionResult addmembers(int sid,employee employee)
        {
            var bmail = Session["email"];
            var b_id = 0;
            if ((string)Session["sp_head"] == "True") //employee
            {
                employee empsingle = emp.employees.Where(x => x.email.Equals(bmail.ToString())).SingleOrDefault();
                b_id = (int)empsingle.b_id;
            }
            else //business
            {
                Business singleData = Spaces.Businesses.Where(x => x.email.Equals(bmail.ToString())).SingleOrDefault();
                b_id = singleData.b_id;
            }
            //drop-down refill
            var names = emp.employees.Where(c => c.b_id == b_id && c.s_id == null).Select(c => new { c.e_id, c.name }).ToList();
            ViewBag.namelist = names;

            //adding employee
            var singleemp = emp.employees.Where(x => x.e_id.Equals(employee.e_id)).SingleOrDefault();
            var space_id = Convert.ToInt32(sid);
            singleemp.s_id = space_id;
            emp.Entry(singleemp).State = System.Data.Entity.EntityState.Modified;
            emp.SaveChanges();

            if(singleemp.empl_type == "Freelancer")
            {
                var fdata = Spaces.freelancers.Where(x => x.email == singleemp.email).SingleOrDefault();
                fdata.s_id = space_id;
                Spaces.Entry(fdata).State = System.Data.Entity.EntityState.Modified;
                Spaces.SaveChanges();
            }
            return Content("<script language='javascript' type='text/javascript'>alert('Employee Added!');</script>");
            
        }
        


        [Route("space/firemembers/{sid}")]
        public ActionResult firemembers(int sid)
        {

            if ((string)Session["sp_head"] == "True" || (string)Session["actype"] == "business")
            {
                var bmail = Session["email"];
                var b_id = 0;
                if ((string)Session["sp_head"] == "True") //employee
                {
                    employee empsingle = emp.employees.Where(x => x.email.Equals(bmail.ToString())).SingleOrDefault();
                    b_id = (int)empsingle.b_id;
                }
                else //business
                {
                    Business singleData = Spaces.Businesses.Where(x => x.email.Equals(bmail.ToString())).SingleOrDefault();
                    b_id = singleData.b_id;
                }

                var names = emp.employees.Where(c => c.b_id == b_id && c.s_id == sid).Select(c => new { c.e_id, c.name }).ToList();
                ViewBag.namelist = names;

                return View();
            }
            else
            {
                return RedirectToAction("restricted", "Space");
            }
        }


        [Route("space/firemembers/{sid}")]
        [HttpPost]
        public ActionResult firemembers(int sid,employee employee)
        {
            var bmail = Session["email"];
            var b_id = 0;
            if ((string)Session["sp_head"] == "True") //employee
            {
                employee empsingle = emp.employees.Where(x => x.email.Equals(bmail.ToString())).SingleOrDefault();
                b_id = (int)empsingle.b_id;
            }
            else //business
            {
                Business singleData = Spaces.Businesses.Where(x => x.email.Equals(bmail.ToString())).SingleOrDefault();
                b_id = singleData.b_id;
            }
            //drop-down refill
            var names = emp.employees.Where(c => c.b_id == b_id && c.s_id == null).Select(c => new { c.e_id, c.name }).ToList();
            ViewBag.namelist = names;

            //removing employee
            employee empdata = emp.employees.Where(x => x.e_id == employee.e_id && x.s_id == sid).SingleOrDefault();
            empdata.s_id = null;
            emp.Entry(empdata).State = System.Data.Entity.EntityState.Modified;
            emp.SaveChanges();
            return Content("<script language='javascript' type='text/javascript'>alert('Member Removed!');</script>");

        }

        [Route("space/deletesapce/{sid}")]
        public ActionResult deletespace(int sid)
        {
            if ((string)Session["actype"] == "business")
            {
                //verifying sid with bid
                var bmail = Session["email"];
                Business singleData = Spaces.Businesses.Where(x => x.email.Equals(bmail.ToString())).SingleOrDefault();
                var b_id = singleData.b_id;
                var bid = Spaces.spaces.Where(x => x.s_id == sid && x.b_id == b_id).FirstOrDefault();
                if(bid != null)
                {
                    space spdata = Spaces.spaces.Where(x => x.s_id == sid).FirstOrDefault();
                    ViewData["sname"] = spdata.s_name;
                    ViewData["sid"] = spdata.s_id;

                    return View();
                }
                else
                {
                    return RedirectToAction("restricted", "Space");
                }

                   
            }
            else
            {
                return RedirectToAction("restricted","Space");
            }
           
        }

        [Route("space/deletesapce/{sid}")]
        [HttpPost]
        public ActionResult deletespace(int sid,employee employee)
        {
            var bmail = Session["email"];
            Business singleData = Spaces.Businesses.Where(x => x.email.Equals(bmail.ToString())).SingleOrDefault();
            var b_id = singleData.b_id;

            //updating employee table
            emp.employees.Where(x => x.s_id == sid).ToList().ForEach(x =>
            {
                x.s_id = null;
            });
            emp.SaveChanges();

            //updating freelancer table
            Spaces.freelancers.Where(x => x.s_id == sid).ToList().ForEach(x =>
            {
                x.s_id = null;
            });
            Spaces.SaveChanges();

            //deleting from tasks table
            Spaces.tasks.RemoveRange(Spaces.tasks.Where(x => x.s_id == sid));
            Spaces.SaveChanges();

            //deleting from issues table
            Spaces.issues.RemoveRange(Spaces.issues.Where(x => x.s_id == sid));
            Spaces.SaveChanges();

            //deleting from space table
            var remspace = Spaces.spaces.Where(x => x.s_id == sid).SingleOrDefault();
            if (remspace != null)
            {
                Spaces.spaces.Remove(remspace);
                Spaces.SaveChanges();
            }

            return RedirectToAction("Index","Business");
        }

        [Route("space/addtasks/{sid}")]
        public ActionResult addtasks(int sid)
        {
            //verification for sid req!
            if ((string)Session["actype"]=="business" || (string)Session["sp_head"] == "True")
            {
                var bmail = Session["email"];
                var b_id = 0;
                if ((string)Session["sp_head"] == "True") //employee
                {
                    employee empsingle = emp.employees.Where(x => x.email.Equals(bmail.ToString())).SingleOrDefault();
                    b_id = (int)empsingle.b_id;
                }
                else //business
                {
                    Business singleData = Spaces.Businesses.Where(x => x.email.Equals(bmail.ToString())).SingleOrDefault();
                    b_id = singleData.b_id;
                }
                var empdata = emp.employees.Where(x => x.b_id == b_id && x.s_id == sid).ToList();
                ViewBag.namelist = empdata;
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
            
        }

        [HttpPost]
        [Route("space/addtasks/{sid}")]
        public ActionResult addtasks(int sid,task task)
        {
            var bmail = Session["email"];
            var b_id = 0;
            if ((string)Session["sp_head"] == "True") //employee
            {
                employee empsingle = emp.employees.Where(x => x.email.Equals(bmail.ToString())).SingleOrDefault();
                b_id = (int)empsingle.b_id;
            }
            else //business
            {
                Business singleData = Spaces.Businesses.Where(x => x.email.Equals(bmail.ToString())).SingleOrDefault();
                b_id = singleData.b_id;
            }

            var names = emp.employees.Where(c => c.b_id == b_id && c.s_id == sid).ToList();
            ViewBag.namelist = names;
            
            //adding task to db
            task db_task = new task();
            db_task.e_id = task.e_id;
            db_task.task_desc = task.task_desc;
            db_task.status = "Ongoing";
            db_task.s_id = Convert.ToInt32(sid);


            Spaces.tasks.Add(db_task);
            Spaces.SaveChanges();

            return Content("<script language='javascript' type='text/javascript'>alert('Task Assigned!');</script>");
            //drop-down refill

        }

        [Route("/space/taskcomplete/{sid,tid}")]
        public ActionResult taskcomplete(int sid,int tid)
        {
            try
            {
                task tupdate = Spaces.tasks.Where(x =>x.s_id == sid && x.t_id == tid).SingleOrDefault();
                tupdate.status = "Completed";
                Spaces.Entry(tupdate).State = System.Data.Entity.EntityState.Modified;
                Spaces.SaveChanges();
                
                return Content("<script language='javascript' type='text/javascript'>alert('Task Completed!');</script>");
            }
            catch
            {
                return Content("<script language='javascript' type='text/javascript'>alert('Error!');</script>");
            }
            
        }

        [Route("space/raiseissue/{sid}")]
        public ActionResult raiseissue(int sid)
        {
            //verification of space_id
            var bmail = Session["email"];
            var b_id = 0;
            if ((string)Session["sp_head"] == "True") //employee
            {
                employee empsingle = emp.employees.Where(x => x.email.Equals(bmail.ToString())).SingleOrDefault();
                b_id = (int)empsingle.b_id;
            }
            else //business
            {
                Business singleData = Spaces.Businesses.Where(x => x.email.Equals(bmail.ToString())).SingleOrDefault();
                b_id = singleData.b_id;
            }
            var empdata = emp.employees.Where(x => x.b_id == b_id && x.s_id == sid).ToList();
            ViewBag.namelist = empdata;
            return View();
        }

        [HttpPost]
        [Route("space/raiseissue/{sid}")]
        public ActionResult raiseissue(int sid,issue issue)
        {
            var bmail = Session["email"];
            var b_id = 0;
            if ((string)Session["sp_head"] == "True") //employee
            {
                employee empsingle = emp.employees.Where(x => x.email.Equals(bmail.ToString())).SingleOrDefault();
                b_id = (int)empsingle.b_id;
            }
            else //business
            {
                Business singleData = Spaces.Businesses.Where(x => x.email.Equals(bmail.ToString())).SingleOrDefault();
                b_id = singleData.b_id;
            }

            var empdata = emp.employees.Where(x => x.b_id == b_id && x.s_id == sid).ToList();
            ViewBag.namelist = empdata;

            issue is_data = new issue();
            is_data.priority = issue.priority;
            is_data.issue1 = issue.issue1;
            is_data.e_id = issue.e_id;
            is_data.s_id = Convert.ToInt32(sid);

            Spaces.issues.Add(is_data);
            Spaces.SaveChanges();

            return Content("<script language='javascript' type='text/javascript'>alert('Issue Raised!');</script>");
        }

        [Route("space/viewissue/{sid}")]
        public ActionResult viewissue(int sid)
        {
            var is_data = Spaces.issues.Where(x => x.s_id == sid).ToList();
            ViewData["is_data"] = is_data;

            return View(is_data);
        }

        
        [Route("space/completeissue/{iid}")]
        public ActionResult completeissue(int iid)
        {
            try
            {
                issue iupdate = Spaces.issues.Where(x => x.i_id == iid).SingleOrDefault();
                if (iupdate != null)
                {
                    Spaces.issues.Remove(iupdate);
                    Spaces.SaveChanges();
                }
          

                return Content("<script language='javascript' type='text/javascript'>alert('Issue Solved!');</script>");
            }
            catch
            {
                return Content("<script language='javascript' type='text/javascript'>alert('Error!');</script>");
            }
        }

        public ActionResult setstatus()
        {
            if ((string)Session["sp_head"] == "True")
            {
                var email = Session["email"];

                var bdata = emp.employees.Where(x => x.email == (string)email).SingleOrDefault();
                var bid = bdata.b_id;
                var sdata = Spaces.spaces.Where(x => x.b_id == bid && x.head_e_id == bdata.e_id).SingleOrDefault();
                ViewData["sid"] = sdata.s_id;
                ViewData["sname"] = sdata.s_name;
                return View();

            }
            else
            {
                return RedirectToAction("index", "home");
            }
            
        }

        [HttpPost]
        public ActionResult setstatus(spaceModel space)
        {

            //update space
            space sdata = Spaces.spaces.Where(x => x.s_id == space.s_id).SingleOrDefault();
            sdata.s_status = space.s_status;
            Spaces.Entry(sdata).State = System.Data.Entity.EntityState.Modified;
            Spaces.SaveChanges();
            return RedirectToAction("Index","Account");

        }



        public ActionResult restricted()
        {
            return View();
        }










    }
}