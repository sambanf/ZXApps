using Asp.NETMVCCRUD.Models;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Asp.NETMVCCRUD.Class;

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
                          join trans in db.tt_Transaction on daily.Daily_PK equals trans.Daily_FK into gj
                          from subpet in gj.DefaultIfEmpty()
                          where daily.Status_FK == 1 && daily.Date.ToString() == date
                          select new Sheet
                          {
                              Daily_PK = daily.Daily_PK,
                              SheetNum = daily.SheetNum,
                              Recorder = record.Nama,
                              HasilKain = db.tt_TransactionDetail.Where(t => t.Transaction_FK == subpet.Transaction_PK && t.Status_FK == 1 && subpet.Status_FK == 1).Sum(i => (Double?)i.HasilKain) ?? 0,
                              Penambahan = subpet.Status_FK == 1 ? subpet.Penambahan ?? 0 : 0,
                              TotalKain = db.tt_TransactionDetail.Where(t => t.Transaction_FK == subpet.Transaction_PK && t.Status_FK == 1 && subpet.Status_FK == 1).Sum(i => (Double?)i.HasilKain) + subpet.Penambahan ?? 0
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

                return Json(new { success = false, message = "Nomor Kertas Sudah di Input" }, JsonRequestBehavior.AllowGet);
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

        public ActionResult Download(string date)
        {
            List<ReportPerMesin> result = new List<ReportPerMesin>();
            using (HELLOWEntities db = new HELLOWEntities())
            {
                result = (from daily in db.tt_Daily
                          join record in db.tm_Recorder on daily.Recorder_FK equals record.Recorder_PK
                          join trans in db.tt_Transaction on daily.Daily_PK equals trans.Daily_FK into gj
                          from subpet in gj.DefaultIfEmpty()
                          join mesin in db.tm_Mesin on subpet.Mesin_FK equals mesin.Mesin_PK
                          join kw in db.tm_KodeWarna on subpet.KodeWarna_FK equals kw.KodeWarna_PK
                          where daily.Status_FK == 1 && daily.Date.ToString() == date
                          select new ReportPerMesin
                          {
                              NoMesin = mesin.KodeMesin,
                              KodeWarna = kw.KodeWarna,
                              HasilKain = db.tt_TransactionDetail.Where(t => t.Transaction_FK == subpet.Transaction_PK && t.Status_FK == 1 && subpet.Status_FK == 1).Sum(i => (Double?)i.HasilKain) ?? 0,
                              Penambahan = subpet.Status_FK == 1 ? subpet.Penambahan ?? 0 : 0,
                              TotalKain = db.tt_TransactionDetail.Where(t => t.Transaction_FK == subpet.Transaction_PK && t.Status_FK == 1 && subpet.Status_FK == 1).Sum(i => (Double?)i.HasilKain) + subpet.Penambahan ?? 0
                          }).GroupBy(l => new { 
                              l.NoMesin,
                              l.KodeWarna
                          })
                          .Select(cl => new ReportPerMesin
                          {
                              NoMesin = cl.FirstOrDefault().NoMesin,
                              KodeWarna = cl.FirstOrDefault().KodeWarna,
                              HasilKain = cl.Sum(x => x.HasilKain),
                              Penambahan = cl.Sum(x => x.Penambahan),
                              TotalKain = cl.Sum(x => x.TotalKain)
                          }).ToList();
            }

            XLWorkbook wb = new XLWorkbook();
            DataTable dt = DataCommonHelper.ConvertListToDataTable(result, string.Empty);
            wb.Worksheets.Add(dt, "Report");
            IXLWorksheet ws = wb.Worksheet(1);
            ws.Columns().AdjustToContents();
            string myName = Server.UrlEncode("ReportPerMesin"+ date +".xlsx");
            MemoryStream stream = GetStream(wb);// The method is defined below
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition",
            "attachment; filename=" + myName);
            Response.ContentType = "application/vnd.ms-excel";
            Response.BinaryWrite(stream.ToArray());
            Response.End();
            return View();
        }

        public MemoryStream GetStream(XLWorkbook excelWorkbook)
        {
            MemoryStream fs = new MemoryStream();
            excelWorkbook.SaveAs(fs);
            fs.Position = 0;
            return fs;
        }

        [HttpGet]
        public ActionResult Edit(int id = 0)
        {
            List<DDLInspector> result = new List<DDLInspector>();
            tt_DailyView edited = new tt_DailyView();
            using (HELLOWEntities db = new HELLOWEntities())
            {
                result = (from inspect in db.tm_Recorder
                           where inspect.Status_FK == 1
                           select new DDLInspector
                           {
                               inspectpk = inspect.Recorder_PK,
                               Text = inspect.Nama + "(" + inspect.NoRecorder + ")"
                           }).ToList();
                ViewBag.Testlist = result;

                edited = (from dail in db.tt_Daily
                          where dail.Daily_PK == id
                          select new tt_DailyView
                          {
                              Daily_PK = dail.Daily_PK,
                              Datetemp = dail.Date,
                              Recorder_FK = dail.Recorder_FK,
                              SheetNum = dail.SheetNum

                          }).FirstOrDefault();

                edited.Date = edited.Datetemp.ToString("yyyy/MM/dd");

                return PartialView(edited);
            }
        }

        [HttpPost]
        public ActionResult Edit(tt_DailyView td)
        {
            DateTime temp;
            using (HELLOWEntities db = new HELLOWEntities())
            {
                tt_Daily dail = db.tt_Daily.Where(x => x.Daily_PK == td.Daily_PK).FirstOrDefault<tt_Daily>();
                if (DateTime.TryParse(td.Date, out temp))
                {
                    dail.Date = temp;
                    dail.Recorder_FK = td.Recorder_FK;
                    dail.SheetNum = td.SheetNum;
                }
                db.SaveChanges();
                return Json(new { success = true, message = "Updated Successfully" }, JsonRequestBehavior.AllowGet);
            }


        }
    }
}