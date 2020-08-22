using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Asp.NETMVCCRUD.Models;
using System.Data.Entity;

namespace Asp.NETMVCCRUD.Controllers
{
    public class MesinController : Controller
    {
        //
        // GET: /tm_Mesin/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetData()
        {
            using (HELLOWEntities db = new HELLOWEntities())
            {
                List<tm_Mesin> empList = db.tm_Mesin.Where(x => x.Status_FK == 1).ToList<tm_Mesin>();
                return Json(new { data = empList }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public ActionResult AddOrEdit(int id = 0)
        {
            if (id == 0)
                return View(new tm_Mesin());
            else
            {
                using (HELLOWEntities db = new HELLOWEntities())
                {
                    return View(db.tm_Mesin.Where(x => x.Mesin_PK==id).FirstOrDefault<tm_Mesin>());
                }
            }
        }

        [HttpPost]
        public ActionResult AddOrEdit(tm_Mesin emp)
        {
            using (HELLOWEntities db = new HELLOWEntities())
            {
                if (emp.Mesin_PK == 0)
                {
                    emp.Status_FK = 1;
                    db.tm_Mesin.Add(emp);
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
                tm_Mesin emp = db.tm_Mesin.Where(x => x.Mesin_PK == id).FirstOrDefault<tm_Mesin>();
                emp.Status_FK = 2;
                db.SaveChanges();
                return Json(new { success = true, message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}