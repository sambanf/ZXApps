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
    
    public partial class tt_TransactionDetail
    {
        public int TransactionDetail_PK { get; set; }
        public int Transaction_FK { get; set; }
        public int KodeWarna_FK { get; set; }
        public int Operator_FK { get; set; }
        public double HasilKain { get; set; }
        public int Status_FK { get; set; }
    }
}
