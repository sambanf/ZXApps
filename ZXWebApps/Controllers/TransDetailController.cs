using ZXWebApps.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ZXWebApps.Controllers
{
    public class TransDetailController : Controller
    {
        // GET: TransDetail
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
                    TransDetailView transDetailView = (from trans in db.tt_Transaction
                                                       join mesin in db.tm_Mesin on trans.Mesin_FK equals mesin.Mesin_PK
                                                       join statmesin in db.tm_StatusMesin on mesin.StatusMesin_FK equals statmesin.StatusMesin_PK
                                                       join daily in db.tt_Daily on trans.Daily_FK equals daily.Daily_PK
                                                       join kodewarna in db.tm_KodeWarna on trans.KodeWarna_FK equals kodewarna.KodeWarna_PK
                                                       where trans.Status_FK == 1 && trans.Transaction_PK == id
                                                       select new TransDetailView
                                                       {
                                                           Tanggal = daily.Date.ToString(),
                                                           transfk = trans.Transaction_PK,
                                                           mesin = mesin.KodeMesin + "(" + statmesin.Status + ")",
                                                           kodewarna = kodewarna.KodeWarna
                                                       }).FirstOrDefault();
                    return View(transDetailView);
                }
            }
        }

        public ActionResult GetData(int id)
        {
            using (HELLOWEntities db = new HELLOWEntities())
            {
                List<TransDetailList> transdetaillist = (from transdetail in db.tt_TransactionDetail
                                                         join trans in db.tt_Transaction on transdetail.Transaction_FK equals trans.Transaction_PK
                                                         join oper in db.tm_Operator on transdetail.Operator_FK equals oper.Operator_PK
                                                         where transdetail.Status_FK == 1 && transdetail.Transaction_FK == id
                                                         select new TransDetailList
                                                         {
                                                             TransDetailPK = transdetail.TransactionDetail_PK,
                                                             NoOperator = oper.NoOperator.ToString(),
                                                             NamaOp = oper.Nama,
                                                             HasilKain = transdetail.HasilKain,
                                                         }).ToList();
                return Json(new { data = transdetaillist }, JsonRequestBehavior.AllowGet);
            }
        }



        [HttpGet]
        public ActionResult AddOrEdit(int id = 0)
        {
            List<DDLOperator> result = new List<DDLOperator>();
            if (id == 0)
            {
                using (HELLOWEntities db = new HELLOWEntities())
                {
                    result = (from oper in db.tm_Operator
                              where oper.Status_FK == 1
                              select new DDLOperator
                              {
                                  operatorpk = oper.Operator_PK,
                                  Text = oper.NoOperator + " - " + oper.Nama
                              }).ToList();
                    ViewBag.OperatorList =result;

                    return View(new tt_TransactionDetail());
                }
            }
            else
            {
                using (HELLOWEntities db = new HELLOWEntities())
                {
                    result = (from oper in db.tm_Operator
                              where oper.Status_FK == 1
                              select new DDLOperator
                              {
                                  operatorpk = oper.Operator_PK,
                                  Text = oper.NoOperator + " - " + oper.Nama
                              }).ToList();
                    ViewBag.OperatorList = result;

                    return View(db.tt_TransactionDetail.Where(x => x.TransactionDetail_PK == id).FirstOrDefault<tt_TransactionDetail>());
                }
            }
        }

        [HttpPost]
        public ActionResult AddOrEdit(tt_TransactionDetail td)
        {
            using (HELLOWEntities db = new HELLOWEntities())
            {
                if (td.TransactionDetail_PK == 0)
                {
                    td.Status_FK = 1;
                    db.tt_TransactionDetail.Add(td);
                    db.SaveChanges();
                    return Json(new { success = true, message = "Saved Successfully" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    td.Status_FK = 1;
                    db.Entry(td).State = EntityState.Modified;
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
                tt_TransactionDetail emp = db.tt_TransactionDetail.Where(x => x.TransactionDetail_PK == id).FirstOrDefault<tt_TransactionDetail>();
                emp.Status_FK = 2;
                db.SaveChanges();
                return Json(new { success = true, message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}