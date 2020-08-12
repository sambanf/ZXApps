using Asp.NETMVCCRUD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Asp.NETMVCCRUD.Controllers
{
    public class TransDetailController : Controller
    {
        // GET: TransDetail
        public ActionResult MainPage(int id)
        {
            if (id == 0 || id == null)
            {
                return RedirectToAction("Index","Dashboard"); 
            }
            else
            {

                return View();
            }
            
        }

        public ActionResult GetData()
        {
            using (DBEntities db = new DBEntities())
            {
                List<tm_Operator> empList = db.tm_Operator.Where(x => x.Status_FK == 1).ToList<tm_Operator>();
                return Json(new { data = empList }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}