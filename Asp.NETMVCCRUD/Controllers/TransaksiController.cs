using Asp.NETMVCCRUD.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Asp.NETMVCCRUD.Controllers
{
    public class TransaksiController : Controller
    {
        // GET: Transaksi
        public ActionResult Index()
        {
            return View();
        }


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

                return PartialView();
            }
        }

        [HttpPost]
        public ActionResult Create(string result)
        {
            InputTransaksi input = new JavaScriptSerializer().Deserialize<InputTransaksi>(result);
            using (HELLOWEntities db = new HELLOWEntities())
            {
                tt_Transaction transaction = new tt_Transaction();
                transaction.Mesin_FK = input.mesin;
                transaction.Daily_FK = input.daily;
                transaction.KodeWarna_FK = input.kodewarna;
                transaction.Status_FK = 1;
                transaction.Penambahan = input.Penambahan;
                transaction.CreatedDate = DateTime.Now;
                db.tt_Transaction.Add(transaction);
                db.SaveChanges();

                foreach (var item in input.transdetail)
                {
                    tt_TransactionDetail tdet = new tt_TransactionDetail();
                    tdet.Transaction_FK = transaction.Transaction_PK;
                    tdet.Operator_FK = item.nooperator;
                    tdet.HasilKain = item.hasil;
                    tdet.Status_FK = 1;
                    db.tt_TransactionDetail.Add(tdet);
                    db.SaveChanges();
                }

                return Json(new { success = true, message = "Saved Successfully" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult MainPage(int id)
        {
            if (id == 0)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            else
            {
                using (HELLOWEntities db = new HELLOWEntities())
                {
                    Daily transDetailView = (from daily in db.tt_Daily
                                             join record in db.tm_Recorder on daily.Recorder_FK equals record.Recorder_PK
                                             where daily.Status_FK == 1 && daily.Daily_PK == id
                                             select new Daily
                                             {
                                                 Daily_PK = daily.Daily_PK,
                                                 Inspector = record.Nama,
                                                 SheetNum = daily.SheetNum
                                             }).FirstOrDefault();
                    return View(transDetailView);
                }
            }
        }

        public ActionResult GetData(int id)
        {
            List<Transaksi> result = new List<Transaksi>();
            using (HELLOWEntities db = new HELLOWEntities())
            {
                result = (from trans in db.tt_Transaction
                          join mesin in db.tm_Mesin on trans.Mesin_FK equals mesin.Mesin_PK
                          join kw in db.tm_KodeWarna on trans.KodeWarna_FK equals kw.KodeWarna_PK
                          where trans.Status_FK == 1 && trans.Daily_FK == id
                          select new Transaksi
                          {
                              Transaction_PK = trans.Transaction_PK,
                              KodeMesin = mesin.KodeMesin.ToString(),
                              KodeWarna = kw.KodeWarna,
                              HasilKain = db.tt_TransactionDetail.Where(t => t.Transaction_FK == trans.Transaction_PK && t.Status_FK == 1).Sum(i => (Double?)i.HasilKain) ?? 0,
                              Penambahan = trans.Penambahan ?? 0,
                              TotalBaris = db.tt_TransactionDetail.Where(t => t.Transaction_FK == trans.Transaction_PK && t.Status_FK == 1).Sum(i => (Double?)i.HasilKain) + trans.Penambahan ?? 0
                          }).ToList();
            }
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            using (HELLOWEntities db = new HELLOWEntities())
            {
                tt_Transaction hehe = db.tt_Transaction.Where(x => x.Transaction_PK == id).FirstOrDefault();
                hehe.Status_FK = 2;
                db.SaveChanges();
                return Json(new { success = true, message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public ActionResult Edit(int id = 0)
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

                return PartialView(db.tt_Transaction.Where(x => x.Transaction_PK == id).FirstOrDefault<tt_Transaction>());
            }
        }

        [HttpPost]
        public ActionResult Edit(tt_Transaction td)
        {
            using (HELLOWEntities db = new HELLOWEntities())
            {
                td.Status_FK = 1;
                db.Entry(td).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, message = "Updated Successfully" }, JsonRequestBehavior.AllowGet);
            }


        }





    }
}