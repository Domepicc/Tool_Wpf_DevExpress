﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Tool_Wpf_DevExpress.Model
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class CostoGiornalieroEncodedEquipmentEntities : DbContext
    {
        public CostoGiornalieroEncodedEquipmentEntities()
            : base("name=CostogiornalieroEncodedEquipmentEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<CostoGiornalieroEncodedEquipment> CostoGiornalieroEncodedEquipment { get; set; }
    }
}