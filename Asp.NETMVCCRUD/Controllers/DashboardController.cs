using Asp.NETMVCCRUD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
            List<Transaksi> result = new List<Transaksi>();
            using (DBEntities db = new DBEntities())
            {
                result = (from trans in db.tt_Transaction
                          join mesin in db.tm_Mesin on trans.Mesin_FK equals mesin.Mesin_PK
                          join statmesin in db.tm_StatusMesin on mesin.StatusMesin_FK equals statmesin.StatusMesin_PK
                          join daily in db.tt_Daily on trans.Daily_FK equals daily.Daily_PK
                          where trans.Status_FK == 1 && daily.Date.ToString() == date
                          select new Transaksi
                          {
                              Mesin_PK = trans.Mesin_FK,
                              KodeMesin = mesin.KodeMesin,
                              StatusMesin = statmesin.Status
                          }).ToList();
               
            }
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult Create(string date)
        {
            using (DBEntities db = new DBEntities())
            {
                tt_Daily dailcheck = new tt_Daily();
                dailcheck = db.tt_Daily.Where(x => x.Date.ToString() == date).FirstOrDefault();
                if (dailcheck == null)
                {
                    tt_Daily dail = new tt_Daily();
                    dail.Date = Convert.ToDateTime(date);
                    dail.Status_FK = 1;

                    db.tt_Daily.Add(dail);
                    db.SaveChanges();
                    dailcheck = db.tt_Daily.Where(x => x.Date.ToString() == date).FirstOrDefault();
                }
                return Json(new { data = dailcheck.Daily_PK }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}