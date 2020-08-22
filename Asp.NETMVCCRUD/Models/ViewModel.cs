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

    public class DDLKodeWarna
    {
        public int kodewarna { get; set; }
        public string Text { get; set; }
    }

    public class DDLOperator
    {
        public int operatorpk { get; set; }
        public string Text { get; set; }
    }

    public class Transaksi
    {
        public int Transaction_PK { get; set; }
        public int Mesin_PK { get; set; }
        public int KodeMesin { get; set; }
        public string StatusMesin { get; set; }
        public string KodeWarna { get; set; }

    }

    public class InputTransaksi
    {
        public int mesin { get; set; }
        public string daily { get; set; } // date
        public int kodewarna { get; set; }
    }
    public class InputTransaksiDetail
    {
        public int mesin { get; set; }
        public string daily { get; set; } // date
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


}