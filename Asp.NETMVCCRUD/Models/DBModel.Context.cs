﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class HELLOWEntities : DbContext
    {
        public HELLOWEntities()
            : base("name=HELLOWEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<am_Status> am_Status { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<tm_KodeWarna> tm_KodeWarna { get; set; }
        public virtual DbSet<tm_Mesin> tm_Mesin { get; set; }
        public virtual DbSet<tm_Operator> tm_Operator { get; set; }
        public virtual DbSet<tm_StatusMesin> tm_StatusMesin { get; set; }
        public virtual DbSet<tt_Daily> tt_Daily { get; set; }
        public virtual DbSet<tt_Transaction> tt_Transaction { get; set; }
        public virtual DbSet<tt_TransactionDetail> tt_TransactionDetail { get; set; }
    }
}
