//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Asp.NETMVCCRUD.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class tt_Transaction
    {
        public int Transaction_PK { get; set; }
        public int Daily_FK { get; set; }
        public int Mesin_FK { get; set; }
        public int KodeWarna_FK { get; set; }
        public int Status_FK { get; set; }
        public Nullable<double> Penambahan { get; set; }
        public int Recorder_FK { get; set; }
        public string SheetNum { get; set; }
    }
}
