using Asp.NETMVCCRUD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Asp.NETMVCCRUD.Controllers
{
    public class DashboardController : Controller
    {
        // GET: Dashboard
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetData(string date)
        {
            List<Sheet> result = new List<Sheet>();
            using (HELLOWEntities db = new HELLOWEntities())
            {
                result = (from daily in db.tt_Daily
                          join record in db.tm_Recorder on daily.Recorder_FK equals record.Recorder_PK
                          join trans in db.tt_Transaction on  daily.Daily_PK equals trans.Daily_FK into gj
                          from subpet in gj.DefaultIfEmpty()
                          where daily.Status_FK == 1 && daily.Date.ToString() == date
                          select new Sheet
                          {
                              Daily_PK = daily.Daily_PK,
                              SheetNum = daily.SheetNum,
                              Recorder = record.Nama,
                              HasilKain = db.tt_TransactionDetail.Where(t => t.Transaction_FK == subpet.Transaction_PK && t.Status_FK == 1).Sum(i => (Double?)i.HasilKain) ?? 0,
                              Penambahan = subpet.Penambahan ?? 0,
                              TotalKain =  db.tt_TransactionDetail.Where(t=>t.Transaction_FK == subpet.Transaction_PK && t.Status_FK == 1).Sum(i => (Double?)i.HasilKain) + subpet.Penambahan ?? 0
                          }).GroupBy(l => l.Daily_PK)
                          .Select(cl => new Sheet
                          {
                              Daily_PK = cl.FirstOrDefault().Daily_PK,
                              SheetNum = cl.FirstOrDefault().SheetNum,
                              Recorder = cl.FirstOrDefault().Recorder,
                              HasilKain = cl.Sum(x => x.HasilKain),
                              Penambahan = cl.Sum(x => x.Penambahan),
                              TotalKain = cl.Sum(x => x.TotalKain)
                          }).ToList();
            }
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult Create()
        {
            List<DDLMesin> result = new List<DDLMesin>();
            List<DDLKodeWarna> result2 = new List<DDLKodeWarna>();
            List<DDLInspector> result3 = new List<DDLInspector>();
            using (HELLOWEntities db = new HELLOWEntities())
            {
                result = (from mesin in db.tm_Mesin
                          join statmesin in db.tm_StatusMesin on mesin.StatusMesin_FK equals statmesin.StatusMesin_PK
                          where mesin.Status_FK == 1 && statmesin.Status_FK == 1
                          select new DDLMesin
                          {
                              mesin = mesin.Mesin_PK,
                              Text = mesin.KodeMesin + " - " + statmesin.Status
                          }).ToList();
                //.Where(p=>!db.tt_Transaction.Where(r=>r.Daily_FK == x).Select(q=>q.Mesin_FK).Contains(p.mesin)).ToList();
                ViewBag.Testlist = result;

                result2 = (from kodewar in db.tm_KodeWarna
                           where kodewar.Status_FK == 1
                           select new DDLKodeWarna
                           {
                               kodewarna = kodewar.KodeWarna_PK,
                               Text = kodewar.KodeWarna + "(" + kodewar.Pick + ")"
                           }).ToList();
                ViewBag.Testlist2 = result2;

                result3 = (from inspect in db.tm_Recorder
                           where inspect.Status_FK == 1
                           select new DDLInspector
                           {
                               inspectpk = inspect.Recorder_PK,
                               Text = inspect.Nama + "(" + inspect.NoRecorder + ")"
                           }).ToList();
                ViewBag.Testlist3 = result3;

                return PartialView();
            }
        }

        [HttpPost]
        public ActionResult CreateDaily(InputDaily input)
        {
            try
            {
                using (HELLOWEntities db = new HELLOWEntities())
                {
                    tt_Daily dail = new tt_Daily();
                    DateTime datehehe = Convert.ToDateTime(input.daily);
                    dail.Date = datehehe;
                    dail.SheetNum = input.sheetnum;
                    dail.Recorder_FK = input.recorder;
                    dail.Status_FK = 1;
                    db.tt_Daily.Add(dail);
                    db.SaveChanges();

                    return Json(new { success = true, message = "Saved Successfully" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = "Nomor Kertas Sudah di Input"}, JsonRequestBehavior.AllowGet);
            }
            
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            using (HELLOWEntities db = new HELLOWEntities())
            {
                tt_Daily emp = db.tt_Daily.Where(x => x.Daily_PK == id).FirstOrDefault<tt_Daily>();
                emp.Status_FK = 2;
                List<tt_Transaction> hehe = db.tt_Transaction.Where(x => x.Daily_FK == id).ToList();
                hehe.ForEach(x => x.Status_FK = 2);
                db.SaveChanges();
                return Json(new { success = true, message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}