using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Asp.NETMVCCRUD.Models;
using System.Data.Entity;

namespace Asp.NETMVCCRUD.Controllers
{
    public class KodeWarnaController : Controller
    {
        //
        // GET: /tm_KodeWarna/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetData()
        {
            using (DBEntities db = new DBEntities())
            {
                List<tm_KodeWarna> empList = db.tm_KodeWarna.Where(x => x.Status_FK == 1).ToList<tm_KodeWarna>();
                return Json(new { data = empList }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public ActionResult AddOrEdit(int id = 0)
        {
            if (id == 0)
                return View(new tm_KodeWarna());
            else
            {
                using (DBEntities db = new DBEntities())
                {
                    return View(db.tm_KodeWarna.Where(x => x.KodeWarna_PK==id).FirstOrDefault<tm_KodeWarna>());
                }
            }
        }

        [HttpPost]
        public ActionResult AddOrEdit(tm_KodeWarna emp)
        {
            using (DBEntities db = new DBEntities())
            {
                if (emp.KodeWarna_PK == 0)
                {
                    emp.Status_FK = 1;
                    db.tm_KodeWarna.Add(emp);
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
            using (DBEntities db = new DBEntities())
            {
                tm_KodeWarna emp = db.tm_KodeWarna.Where(x => x.KodeWarna_PK == id).FirstOrDefault<tm_KodeWarna>();
                emp.Status_FK = 2;
                db.SaveChanges();
                return Json(new { success = true, message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}