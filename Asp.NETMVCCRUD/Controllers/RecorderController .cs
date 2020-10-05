using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Asp.NETMVCCRUD.Models;
using System.Data.Entity;

namespace Asp.NETMVCCRUD.Controllers
{
    public class RecorderController : Controller
    {
        //
        // GET: /tm_Recorder/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetData()
        {
            using (HELLOWEntities db = new HELLOWEntities())
            {
                List<tm_Recorder> empList = db.tm_Recorder.Where(x => x.Status_FK == 1).ToList<tm_Recorder>();
                return Json(new { data = empList }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public ActionResult AddOrEdit(int id = 0)
        {
            if (id == 0)
                return View(new tm_Recorder());
            else
            {
                using (HELLOWEntities db = new HELLOWEntities())
                {
                    return View(db.tm_Recorder.Where(x => x.Recorder_PK==id).FirstOrDefault<tm_Recorder>());
                }
            }
        }

        [HttpPost]
        public ActionResult AddOrEdit(tm_Recorder emp)
        {
            using (HELLOWEntities db = new HELLOWEntities())
            {
                if (emp.Recorder_PK == 0)
                {
                    emp.Status_FK = 1;
                    db.tm_Recorder.Add(emp);
                    db.SaveChanges();
                    return Json(new { success = true, message = "Saved Successfully" }, JsonRequestBehavior.AllowGet);
                }
                else {
                    emp.Status_FK = 1;
                    db.Entry(emp).State = EntityState.Modified;
                    db.SaveChanges();
                    return Json(new { success = true, message = "Updated Successfully" }, JsonRequestBehavior.AllowGet);
                }
            }


        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            using (HELLOWEntities db = new HELLOWEntities())
            {
                tm_Recorder emp = db.tm_Recorder.Where(x => x.Recorder_PK == id).FirstOrDefault<tm_Recorder>();
                emp.Status_FK = 2;
                db.SaveChanges();
                return Json(new { success = true, message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}