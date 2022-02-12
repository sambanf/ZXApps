using ZXWebApps.Class;
using ZXWebApps.Models;
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

namespace ZXWebApps.Controllers
{
    public class ReportAddOnController : Controller
    { 

        //Report Per Kode WArna
        public ActionResult Waving()
        {
            return View();
        }
        public ActionResult GetDataWaving(ReportProperty rp)
        {
            List<ReportPerKodeWarna> result = new List<ReportPerKodeWarna>();
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
                          select new ReportPerKodeWarna
                          {
                              KodeWarna = kodewarna.KodeWarna,
                              HasilKain = td.HasilKain
                          }).GroupBy(l => l.KodeWarna)
                          .Select(cl => new ReportPerKodeWarna
                          {
                              KodeWarna = cl.FirstOrDefault().KodeWarna,
                              HasilKain = cl.Sum(x => x.HasilKain),
                          }).ToList();
            }
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        //Report Per Mesin
        public ActionResult Inspect()
        {
            return View();
        }
        public ActionResult GetDataInspect(ReportProperty rp)
        {
            List<ReportPerMesin> result = new List<ReportPerMesin>();
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
                          select new ReportPerMesin
                          {
                              NoMesin = mesin.KodeMesin,
                              HasilKain = td.HasilKain
                          }).GroupBy(l => l.NoMesin)
                          .Select(cl => new ReportPerMesin
                          {
                              NoMesin = cl.FirstOrDefault().NoMesin,
                              HasilKain = cl.Sum(x => x.HasilKain),
                          }).ToList();
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
            List<ReportAddOnPerMesin> result = new List<ReportAddOnPerMesin>();
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
                          select new ReportAddOnPerMesin
                          {
                              NoMesin = mesin.KodeMesin,
                              HasilKain = td.HasilKain
                          }).GroupBy(l => l.NoMesin)
                           .Select(cl => new ReportAddOnPerMesin
                           {
                               NoMesin = cl.FirstOrDefault().NoMesin,
                               HasilKain = cl.Sum(x => x.HasilKain),
                           }).ToList();
            }

            XLWorkbook wb = new XLWorkbook();
            DataTable dt = DataCommonHelper.ConvertListToDataTable(result, string.Empty);
            wb.Worksheets.Add(dt, "ReportPerMesin");
            IXLWorksheet ws = wb.Worksheet(1);
            ws.Columns().AdjustToContents();
            string myName = Server.UrlEncode("ReportPerMesin.xlsx");
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

        //KodeWarna
        public ActionResult DownloadWaving(ReportProperty rp)
        {
            List<ReportPerKodeWarna> result = new List<ReportPerKodeWarna>();
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
                          select new ReportPerKodeWarna
                          {
                              KodeWarna = kodewarna.KodeWarna,
                              HasilKain = td.HasilKain
                          }).GroupBy(l => l.KodeWarna)
                            .Select(cl => new ReportPerKodeWarna
                            {
                                KodeWarna = cl.FirstOrDefault().KodeWarna,
                                HasilKain = cl.Sum(x => x.HasilKain),
                            }).ToList();
            }

            XLWorkbook wb = new XLWorkbook();
            DataTable dt = DataCommonHelper.ConvertListToDataTable(result, string.Empty);
            wb.Worksheets.Add(dt, "ReportPerKodeWarna");
            IXLWorksheet ws = wb.Worksheet(1);
            ws.Columns().AdjustToContents();
            string myName = Server.UrlEncode("ReportPerKodeWarna.xlsx");
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