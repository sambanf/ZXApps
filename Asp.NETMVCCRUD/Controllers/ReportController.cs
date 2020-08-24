using Asp.NETMVCCRUD.Models;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Asp.NETMVCCRUD.Controllers
{
    public class ReportController : Controller
    {
        // GET: Report
        public ActionResult Index()
        {
            List<DDLOperator> result = new List<DDLOperator>();
            using (HELLOWEntities db = new HELLOWEntities())
            {
                result = (from oper in db.tm_Operator
                          where oper.Status_FK == 1
                          select new DDLOperator
                          {
                              operatorpk = oper.Operator_PK,
                              Text = oper.NIP + " - " + oper.Nama
                          }).ToList();
                ViewBag.OperatorList = new SelectList(result, "operatorpk", "Text");

                return View();
            }
            
        }


        public ActionResult GetData(ReportProperty rp)
        {
            List<ReportList> result = new List<ReportList>();
            using (HELLOWEntities db = new HELLOWEntities())
            {
                result = (from td in db.tt_TransactionDetail
                         join t in db.tt_Transaction on td.Transaction_FK equals t.Transaction_PK
                         join mesin in db.tm_Mesin on t.Mesin_FK equals mesin.Mesin_PK
                         join statmesin in db.tm_StatusMesin on mesin.StatusMesin_FK equals statmesin.StatusMesin_PK
                         join daily in db.tt_Daily on t.Daily_FK equals daily.Daily_PK
                         join kodewarna in db.tm_KodeWarna on t.KodeWarna_FK equals kodewarna.KodeWarna_PK
                         where td.Status_FK == 1 && td.Operator_FK == rp.Operator && daily.Date.Month == rp.Bulan && daily.Date.Year == rp.Tahun
                         select new ReportList
                         {
                             Tanggal = daily.Date.ToString(),
                             KodeWarna = kodewarna.KodeWarna,
                             StatusMesin = statmesin.Status,
                             HasilKain = td.HasilKain,
                             Pick = kodewarna.Pick,
                             Nilai = statmesin.Nilai,
                             HargaMeter = kodewarna.Pick * statmesin.Nilai,
                             Total = kodewarna.Pick * statmesin.Nilai * td.HasilKain
                         }).ToList();
                          
            }
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSum(ReportProperty rp)
        {
            List<ReportList> result = new List<ReportList>();
            using (HELLOWEntities db = new HELLOWEntities())
            {
                result = (from td in db.tt_TransactionDetail
                          join t in db.tt_Transaction on td.Transaction_FK equals t.Transaction_PK
                          join mesin in db.tm_Mesin on t.Mesin_FK equals mesin.Mesin_PK
                          join statmesin in db.tm_StatusMesin on mesin.StatusMesin_FK equals statmesin.StatusMesin_PK
                          join daily in db.tt_Daily on t.Daily_FK equals daily.Daily_PK
                          join kodewarna in db.tm_KodeWarna on t.KodeWarna_FK equals kodewarna.KodeWarna_PK
                          where td.Status_FK == 1 && td.Operator_FK == rp.Operator && daily.Date.Month == rp.Bulan && daily.Date.Year == rp.Tahun
                          select new ReportList
                          {
                              Tanggal = daily.Date.ToString(),
                              KodeWarna = kodewarna.KodeWarna,
                              StatusMesin = statmesin.Status,
                              HasilKain = td.HasilKain,
                              Pick = kodewarna.Pick,
                              Nilai = statmesin.Nilai,
                              HargaMeter = kodewarna.Pick * statmesin.Nilai,
                              Total = kodewarna.Pick * statmesin.Nilai * td.HasilKain
                          }).ToList();
            }
            double summary = 0;
            foreach (var item in result)
            {
                summary += item.Total;
            }
            return Json(new { data = summary }, JsonRequestBehavior.AllowGet);
        }



        [HttpGet]
        public ActionResult AddOrEdit(int id = 0)
        {
            if (id == 0)
                return View(new tm_KodeWarna());
            else
            {
                using (HELLOWEntities db = new HELLOWEntities())
                {
                    return View(db.tm_KodeWarna.Where(x => x.KodeWarna_PK == id).FirstOrDefault<tm_KodeWarna>());
                }
            }
        }

        [HttpPost]
        public ActionResult AddOrEdit(tm_KodeWarna emp)
        {
            using (HELLOWEntities db = new HELLOWEntities())
            {
                if (emp.KodeWarna_PK == 0)
                {
                    emp.Status_FK = 1;
                    db.tm_KodeWarna.Add(emp);
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
                tm_KodeWarna emp = db.tm_KodeWarna.Where(x => x.KodeWarna_PK == id).FirstOrDefault<tm_KodeWarna>();
                emp.Status_FK = 2;
                db.SaveChanges();
                return Json(new { success = true, message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}