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


            List<MesinList> result = new List<MesinList>();

            using (HELLOWEntities db = new HELLOWEntities())
            {

                result = (from mesin in db.tm_Mesin
                          join statmesin in db.tm_StatusMesin on mesin.StatusMesin_FK equals statmesin.StatusMesin_PK
                          where mesin.Status_FK == 1
                          select new MesinList
                          {
                              Mesin_PK = mesin.Mesin_PK,
                              KodeMesin = mesin.KodeMesin,
                              StatusMesin = statmesin.Status
                          }).ToList();

            }
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);

        }


        [HttpGet]
        public ActionResult AddOrEdit(int id = 0)
        {
            using (HELLOWEntities db = new HELLOWEntities())
            {
                List<DDLStatMesin> ddlstatmesin = new List<DDLStatMesin>();
                ddlstatmesin = (from statmesin in db.tm_StatusMesin
                                where statmesin.Status_FK == 1
                                select new DDLStatMesin
                                {
                                    statmesinpk = statmesin.StatusMesin_PK,
                                    Text = statmesin.Status
                                }).ToList();
                ViewBag.DDLStatMesin = ddlstatmesin;
            }
            if (id == 0)
                return View(new tm_Mesin());
            else
            {
                using (HELLOWEntities db = new HELLOWEntities())
                {
                    return View(db.tm_Mesin.Where(x => x.Mesin_PK == id).FirstOrDefault<tm_Mesin>());
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
                else
                {
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