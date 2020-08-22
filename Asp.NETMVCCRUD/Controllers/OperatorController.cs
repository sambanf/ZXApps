using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Asp.NETMVCCRUD.Models;
using System.Data.Entity;

namespace Asp.NETMVCCRUD.Controllers
{
    public class OperatorController : Controller
    {
        //
        // GET: /tm_Operator/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetData()
        {
            using (HELLOWEntities db = new HELLOWEntities())
            {
                List<tm_Operator> empList = db.tm_Operator.Where(x => x.Status_FK == 1).ToList<tm_Operator>();
                return Json(new { data = empList }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public ActionResult AddOrEdit(int id = 0)
        {
            if (id == 0)
                return View(new tm_Operator());
            else
            {
                using (HELLOWEntities db = new HELLOWEntities())
                {
                    return View(db.tm_Operator.Where(x => x.Operator_PK==id).FirstOrDefault<tm_Operator>());
                }
            }
        }

        [HttpPost]
        public ActionResult AddOrEdit(tm_Operator emp)
        {
            using (HELLOWEntities db = new HELLOWEntities())
            {
                if (emp.Operator_PK == 0)
                {
                    emp.Status_FK = 1;
                    db.tm_Operator.Add(emp);
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
                tm_Operator emp = db.tm_Operator.Where(x => x.Operator_PK == id).FirstOrDefault<tm_Operator>();
                emp.Status_FK = 2;
                db.SaveChanges();
                return Json(new { success = true, message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}