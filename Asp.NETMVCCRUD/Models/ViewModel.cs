using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Asp.NETMVCCRUD.Models
{
    public class DDLMesin
    {
        public int mesin { get; set; }
        public string Text { get; set; }
    }

    public class DDLStatMesin
    {
        public int statmesinpk { get; set; }
        public string Text { get; set; }
    }

    public class DDLKodeWarna
    {
        public int kodewarna { get; set; }
        public string Text { get; set; }
    }

    public class DDLInspector
    {
        public int inspectpk { get; set; }
        public string Text { get; set; }
    }



    public class DDLOperator
    {
        public int operatorpk { get; set; }
        public string Text { get; set; }
    }



    public class MesinList
    {
        public int Mesin_PK { get; set; }
        public int KodeMesin { get; set; }
        public string StatusMesin { get; set; }
    }



    public class Transaksi
    {
        public int Transaction_PK { get; set; }
        public int Mesin_PK { get; set; }
        public int KodeMesin { get; set; }
        public string sheetnum { get; set; }
        public string recorder { get; set; }
        public string KodeWarna { get; set; }
        public double total { get; set; }


    }

    public class InputTransaksi
    {
        public int mesin { get; set; }
        public string daily { get; set; } // date
        public int kodewarna { get; set; }
        public int recorder { get; set; }
        public string sheetnum { get; set; }
        public double Penambahan { get; set; }
        public List<InputTransaksiDetail> transdetail { get; set; }
    }
    public class InputTransaksiDetail
    {
        public int nooperator { get; set; }
        public double hasil { get; set; } // date
    }

    public class TransDetailView
    {
        public string Tanggal { get; set; }
        public int transfk { get; set; }
        public string mesin { get; set; }
        public string kodewarna { get; set; } 
    }

    public class TransDetailList
    {
        public int TransDetailPK { get; set; }
        public string NoOperator { get; set; }
        public string NamaOp { get; set; }
        public double HasilKain { get; set; }
    }

    public class ReportList
    {
        public string Tanggal { get; set; }
        public string KodeWarna { get; set; }
        public string StatusMesin { get; set; }
        public double HasilKain { get; set; }
        public int Pick { get; set; }
        public double Nilai { get; set; }
        public double HargaMeter { get; set; }
        public double Total { get; set; }

    }

    public class ReportListInspect
    {
        public string Tanggal { get; set; }
        public string SheetNum { get; set; }
        public string NoMesin { get; set; }
        public double HasilKain { get; set; }
        public double Total { get; set; }
    }
    public class ReportListWaving
    {
        public string NoOperator { get; set; }
        public string NIP { get; set; }
        public string Nama { get; set; }
        public double HasilKain { get; set; }
        public double Total { get; set; }
    }



    public class ReportProperty
    {
        public DateTime startdate { get; set; }
        public DateTime enddate { get; set; }
        public int Operator { get; set; }
    }



}