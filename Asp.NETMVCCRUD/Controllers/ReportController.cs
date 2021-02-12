using Asp.NETMVCCRUD.Class;
using Asp.NETMVCCRUD.Models;
using ClosedXML.Excel;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Web;
using System.Web.Mvc;

namespace Asp.NETMVCCRUD.Controllers
{
    public class ReportController : Controller
    {
        // GET: Report

        //Detail Waving
        public ActionResult DetailWaving()
        {
            List<DDLOperator> result = new List<DDLOperator>();
            using (HELLOWEntities db = new HELLOWEntities())
            {
                result = (from oper in db.tm_Operator
                          where oper.Status_FK == 1
                          select new DDLOperator
                          {
                              operatorpk = oper.Operator_PK,
                              Text = oper.NoOperator + " - " + oper.Nama
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
                          where daily.Status_FK == 1 && t.Status_FK == 1 && td.Status_FK == 1 && td.Operator_FK == rp.Operator && daily.Date >= rp.startdate && daily.Date <= rp.enddate
                          select new ReportList
                          {
                              Tanggal = daily.Date.ToString(),
                              KodeWarna = kodewarna.KodeWarna,
                              StatusMesin = statmesin.Status,
                              HasilKain = td.HasilKain,
                              Pick = kodewarna.Pick,
                              Nilai = statmesin.Nilai,
                              HargaMeter = Math.Round(kodewarna.Pick * statmesin.Nilai, 2),
                              Total = Math.Round(kodewarna.Pick * statmesin.Nilai * td.HasilKain, 2)
                          }).ToList();
                double summary = 0;
                foreach (var item in result)
                {
                    summary += item.Total;
                }
                ViewBag.Summary = summary;
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
                          where daily.Status_FK == 1 && t.Status_FK == 1 && td.Status_FK == 1 && td.Operator_FK == rp.Operator && daily.Date >= rp.startdate && daily.Date <= rp.enddate
                          select new ReportList
                          {
                              Tanggal = daily.Date.ToString(),
                              KodeWarna = kodewarna.KodeWarna,
                              StatusMesin = statmesin.Status,
                              HasilKain = td.HasilKain,
                              Pick = kodewarna.Pick,
                              Nilai = statmesin.Nilai,
                              HargaMeter = Math.Round(kodewarna.Pick * statmesin.Nilai, 2),
                              Total = Math.Round(kodewarna.Pick * statmesin.Nilai * td.HasilKain, 2)
                          }).ToList();
                double summary = 0;
                foreach (var item in result)
                {
                    summary += item.Total;
                }
                summary = Math.Round(summary, 2);
                return Json(new { data = summary }, JsonRequestBehavior.AllowGet);
            }
        }

        //Detail Inspect
        public ActionResult DetailInspect()
        {
            List<DDLInspector> result = new List<DDLInspector>();
            using (HELLOWEntities db = new HELLOWEntities())
            {
                result = (from inspect in db.tm_Recorder
                          where inspect.Status_FK == 1
                          select new DDLInspector
                          {
                              inspectpk = inspect.Recorder_PK,
                              Text = inspect.NoRecorder + " - " + inspect.Nama
                          }).ToList();
                ViewBag.RecorderList = new SelectList(result, "inspectpk", "Text");

                return View();
            }

        }
        public ActionResult GetDataInspectD(ReportProperty rp)
        {
            List<ReportListInspect> result = new List<ReportListInspect>();
            using (HELLOWEntities db = new HELLOWEntities())
            {
                var res = (from t in db.tt_Transaction
                          join mesin in db.tm_Mesin on t.Mesin_FK equals mesin.Mesin_PK
                          join daily in db.tt_Daily on t.Daily_FK equals daily.Daily_PK
                          where daily.Status_FK == 1 && t.Status_FK == 1 && daily.Recorder_FK == rp.Operator && daily.Date >= rp.startdate && daily.Date <= rp.enddate
                          select new ReportListInspect
                          {
                              Tanggal = daily.Date.ToString(),
                              SheetNum = daily.SheetNum,
                              NoMesin = mesin.KodeMesin.ToString(),
                              HasilKain = db.tt_TransactionDetail.Where(x => x.Transaction_FK == t.Transaction_PK).Sum(i => (Double?)i.HasilKain) + (t.Penambahan.HasValue ? t.Penambahan.Value : 0.0) ?? 0
                          }).GroupBy(l => l.SheetNum)
                          .Select(cl => new ReportListInspect
                          {
                              Tanggal = cl.FirstOrDefault().Tanggal,
                              SheetNum = cl.FirstOrDefault().SheetNum,
                              NoMesin = cl.FirstOrDefault().NoMesin,
                              HasilKain = cl.Sum(x => x.HasilKain),
                          }).ToList(); ;

                result = res.ToList();

                foreach (var item in result)
                {
                    item.Total = item.HasilKain * 25;
                }
            }
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetSumInspectD(ReportProperty rp)
        {
            List<ReportListInspect> result = new List<ReportListInspect>();
            using (HELLOWEntities db = new HELLOWEntities())
            {
                result = (from t in db.tt_Transaction
                          join daily in db.tt_Daily on t.Daily_FK equals daily.Daily_PK
                          where daily.Status_FK == 1 && t.Status_FK == 1 && daily.Recorder_FK == rp.Operator && daily.Date >= rp.startdate && daily.Date <= rp.enddate
                          select new ReportListInspect
                          {
                              Tanggal = daily.Date.ToString(),
                              SheetNum = daily.SheetNum,
                              NoMesin = t.Mesin_FK.ToString(),
                              HasilKain = db.tt_TransactionDetail.Where(x => x.Transaction_FK == t.Transaction_PK).Sum(i => (Double?)i.HasilKain) + (t.Penambahan.HasValue ? t.Penambahan.Value : 0.0) ?? 0
                          }).ToList();
                double summary = 0;
                foreach (var item in result)
                {
                    item.Total = item.HasilKain * 25;
                    summary += item.Total;
                }
                summary = Math.Round(summary, 2);
                return Json(new { data = summary }, JsonRequestBehavior.AllowGet);
            }
        }

        //Waving
        public ActionResult Waving()
        {
            return View();
        }
        public ActionResult GetDataWaving(ReportProperty rp)
        {
            List<ReportListWaving> result = new List<ReportListWaving>();
            using (HELLOWEntities db = new HELLOWEntities())
            {
                result = (from td in db.tt_TransactionDetail
                          join t in db.tt_Transaction on td.Transaction_FK equals t.Transaction_PK
                          join mesin in db.tm_Mesin on t.Mesin_FK equals mesin.Mesin_PK
                          join statmesin in db.tm_StatusMesin on mesin.StatusMesin_FK equals statmesin.StatusMesin_PK
                          join daily in db.tt_Daily on t.Daily_FK equals daily.Daily_PK
                          join kodewarna in db.tm_KodeWarna on t.KodeWarna_FK equals kodewarna.KodeWarna_PK
                          join op in db.tm_Operator on td.Operator_FK equals op.Operator_PK
                          where daily.Status_FK == 1 && t.Status_FK == 1 && td.Status_FK == 1 && daily.Date >= rp.startdate && daily.Date <= rp.enddate
                          select new ReportListWaving
                          {
                              NoOperator = op.NoOperator.ToString(),
                              NIP = op.NIP,
                              Nama = op.Nama,
                              HasilKain = td.HasilKain,
                              Total = Math.Round(kodewarna.Pick * statmesin.Nilai * td.HasilKain, 2)
                          }).GroupBy(l => l.NoOperator)
                          .Select(cl => new ReportListWaving
                          {
                              NoOperator = cl.FirstOrDefault().NoOperator,
                              NIP = cl.FirstOrDefault().NIP,
                              Nama = cl.FirstOrDefault().Nama,
                              HasilKain = cl.Sum(x => x.HasilKain),
                              Total = cl.Sum(x => x.Total)
                          }).ToList();
    
            }
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        //Inspect
        public ActionResult Inspect()
        {
            return View();
        }
        public ActionResult GetDataInspect(ReportProperty rp)
        {
            List<ReportListWaving> result = new List<ReportListWaving>();
            using (HELLOWEntities db = new HELLOWEntities())
            {
                result = (from t in db.tt_Transaction
                          join daily in db.tt_Daily on t.Daily_FK equals daily.Daily_PK
                          join rc in db.tm_Recorder on daily.Recorder_FK equals rc.Recorder_PK
                          where daily.Status_FK == 1 && t.Status_FK == 1 && daily.Date >= rp.startdate && daily.Date <= rp.enddate
                          select new ReportListWaving
                          {
                              NoOperator = rc.NoRecorder.ToString(),
                              NIP = rc.NIP,
                              Nama = rc.Nama,
                              HasilKain = db.tt_TransactionDetail.Where(x => x.Transaction_FK == t.Transaction_PK).Sum(i => ((Double?)i.HasilKain) ?? 0) + (t.Penambahan ?? 0.0)
                          }).GroupBy(l => l.NoOperator)
                          .Select(cl => new ReportListWaving
                          {
                              NoOperator = cl.FirstOrDefault().NoOperator,
                              NIP = cl.FirstOrDefault().NIP,
                              Nama = cl.FirstOrDefault().Nama,
                              HasilKain = cl.Sum(x => x.HasilKain),
                          }).ToList();
                foreach (var item in result)
                {
                    item.Total = item.HasilKain * 25;
                }
            }
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        //Print Inspect
        public MemoryStream GetStream(XLWorkbook excelWorkbook)
        {
            MemoryStream fs = new MemoryStream();
            excelWorkbook.SaveAs(fs);
            fs.Position = 0;
            return fs;
        }
        public ActionResult DownloadInspect(ReportProperty rp)
        {
            List<ReportListWaving> result = new List<ReportListWaving>();
            using (HELLOWEntities db = new HELLOWEntities())
            {
                result = (from t in db.tt_Transaction
                          join daily in db.tt_Daily on t.Daily_FK equals daily.Daily_PK
                          join rc in db.tm_Recorder on daily.Recorder_FK equals rc.Recorder_PK
                          where daily.Status_FK == 1 && t.Status_FK == 1 && daily.Date >= rp.startdate && daily.Date <= rp.enddate
                          select new ReportListWaving
                          {
                              NoOperator = rc.NoRecorder.ToString(),
                              NIP = rc.NIP,
                              Nama = rc.Nama,
                              HasilKain = db.tt_TransactionDetail.Where(x => x.Transaction_FK == t.Transaction_PK).Sum(i => ((Double?)i.HasilKain) ?? 0) + (t.Penambahan ?? 0.0)
                          }).GroupBy(l => l.NoOperator)
                          .Select(cl => new ReportListWaving
                          {
                              NoOperator = cl.FirstOrDefault().NoOperator,
                              NIP = cl.FirstOrDefault().NIP,
                              Nama = cl.FirstOrDefault().Nama,
                              HasilKain = cl.Sum(x => x.HasilKain),
                          }).ToList();
                foreach (var item in result)
                {
                    item.Total = item.HasilKain * 25;
                }
            }

            XLWorkbook wb = new XLWorkbook();
            DataTable dt = DataCommonHelper.ConvertListToDataTable(result, string.Empty);
            wb.Worksheets.Add(dt, "GajiInspect");
            IXLWorksheet ws = wb.Worksheet(1);

            string myName = Server.UrlEncode("GajiInspect.xlsx");
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

        public ActionResult DownloadWaving(ReportProperty rp)
        {
            List<ReportListWaving> result = new List<ReportListWaving>();
            using (HELLOWEntities db = new HELLOWEntities())
            {
                result = (from td in db.tt_TransactionDetail
                          join t in db.tt_Transaction on td.Transaction_FK equals t.Transaction_PK
                          join mesin in db.tm_Mesin on t.Mesin_FK equals mesin.Mesin_PK
                          join statmesin in db.tm_StatusMesin on mesin.StatusMesin_FK equals statmesin.StatusMesin_PK
                          join daily in db.tt_Daily on t.Daily_FK equals daily.Daily_PK
                          join kodewarna in db.tm_KodeWarna on t.KodeWarna_FK equals kodewarna.KodeWarna_PK
                          join op in db.tm_Operator on td.Operator_FK equals op.Operator_PK
                          where daily.Status_FK == 1 && t.Status_FK == 1 && td.Status_FK == 1 && daily.Date >= rp.startdate && daily.Date <= rp.enddate
                          select new ReportListWaving
                          {
                              NoOperator = op.NoOperator.ToString(),
                              NIP = op.NIP,
                              Nama = op.Nama,
                              HasilKain = td.HasilKain,
                              Total = Math.Round(kodewarna.Pick * statmesin.Nilai * td.HasilKain, 2)
                          }).GroupBy(l => l.NoOperator)
                          .Select(cl => new ReportListWaving
                          {
                              NoOperator = cl.FirstOrDefault().NoOperator,
                              NIP = cl.FirstOrDefault().NIP,
                              Nama = cl.FirstOrDefault().Nama,
                              HasilKain = cl.Sum(x => x.HasilKain),
                              Total = cl.Sum(x => x.Total)
                          }).ToList();

            }

            XLWorkbook wb = new XLWorkbook();
            DataTable dt = DataCommonHelper.ConvertListToDataTable(result, string.Empty);
            wb.Worksheets.Add(dt, "GajiWaving");
            IXLWorksheet ws = wb.Worksheet(1);

            string myName = Server.UrlEncode("GajiWaving.xlsx");
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

    }

}