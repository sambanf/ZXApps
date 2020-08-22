using Asp.NETMVCCRUD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Asp.NETMVCCRUD.Controllers
{
    public class TransaksiController : Controller
    {
        // GET: Transaksi
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create(int x)
        {
            List<DDLMesin> result = new List<DDLMesin>();
            List<DDLKodeWarna> result2 = new List<DDLKodeWarna>();
            using (HELLOWEntities db = new HELLOWEntities())
            {
                result = (from mesin in db.tm_Mesin
                          join statmesin in db.tm_StatusMesin on mesin.StatusMesin_FK equals statmesin.StatusMesin_PK
                          where mesin.Status_FK == 1 && statmesin.Status_FK == 1 
                          select new DDLMesin
                          {
                              mesin = mesin.Mesin_PK,
                              Text = mesin.KodeMesin + " - " + statmesin.Status
                          }).Where(p=>!db.tt_Transaction.Where(r=>r.Daily_FK == x).Select(q=>q.Mesin_FK).Contains(p.mesin)).ToList();
                ViewBag.Testlist = result;

                result2 = (from kodewar in db.tm_KodeWarna
                          where kodewar.Status_FK == 1 
                          select new DDLKodeWarna
                          {
                              kodewarna = kodewar.KodeWarna_PK,
                              Text = kodewar.KodeWarna +"(" + kodewar.Pick +")"
                          }).ToList();  
                ViewBag.Testlist2 = result2;


                return PartialView();
            }
        }

        [HttpPost]
        public ActionResult Create(InputTransaksi input)
        {
            
            using (HELLOWEntities db = new HELLOWEntities())
            {
                tt_Daily dail = new tt_Daily();
                DateTime datehehe = Convert.ToDateTime(input.daily);
                dail = db.tt_Daily.Where(x => x.Date == datehehe).FirstOrDefault();
                tt_Transaction transaction = new tt_Transaction();
                transaction.Mesin_FK = input.mesin;
                transaction.Daily_FK = dail.Daily_PK;
                transaction.KodeWarna_FK = input.kodewarna;
                transaction.Status_FK = 1;
                db.tt_Transaction.Add(transaction);
                db.SaveChanges();

                return Json(new { success = true, message = "Saved Successfully" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            using (HELLOWEntities db = new HELLOWEntities())
            {
                tt_Transaction emp = db.tt_Transaction.Where(x => x.Transaction_PK == id).FirstOrDefault<tt_Transaction>();
                emp.Status_FK = 2;  
                db.SaveChanges();
                return Json(new { success = true, message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}