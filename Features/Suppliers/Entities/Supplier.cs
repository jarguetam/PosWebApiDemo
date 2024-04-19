using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Pos.WebApi.Features.Suppliers.Entities
{
    public class Supplier
    {
        public int SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string SupplierCode { get; set; }
        public string Rtn { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public int SupplierCategoryId { get; set; }
        public int PayConditionId { get; set; }
        public decimal Balance { get; set; }
        public decimal CreditLine { get; set; }
        public bool Tax { get; set; }
        public int CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public int UpdateBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool Active { get; set; }

        public bool IsValid()
        {
            if (string.IsNullOrEmpty(this.SupplierName)) throw new System.Exception("Debe ingresar un nombre.");
            if (string.IsNullOrEmpty(this.Rtn)) throw new System.Exception("Debe ingresar el RTN.");
            if ((this.SupplierCategoryId) == 0) throw new System.Exception("Debe seleccionar una categoria.");
            if ((this.PayConditionId) == 0) throw new System.Exception("Debe seleccionar una condicion de pago.");
            if ((this.PayConditionId) != 1 && this.CreditLine == 0) throw new System.Exception("Debe ingresar el limite de credito.");
            return true;
        }

        public class Map
        {
            public Map(EntityTypeBuilder<Supplier> builder)
            {
                builder.HasKey(x => x.SupplierId);
                builder.Property(x => x.SupplierId).HasColumnName("SupplierId");
                builder.Property(x => x.SupplierName).HasColumnName("SupplierName");
                builder.Property(x => x.SupplierCode).HasColumnName("SupplierCode");
                builder.Property(x => x.Rtn).HasColumnName("Rtn");
                builder.Property(x => x.Phone).HasColumnName("Phone");
                builder.Property(x => x.Email).HasColumnName("Email");
                builder.Property(x => x.Address).HasColumnName("Address");
                builder.Property(x => x.SupplierCategoryId).HasColumnName("SupplierCategoryId");
                builder.Property(x => x.PayConditionId).HasColumnName("PayConditionId");
                builder.Property(x => x.Balance).HasColumnName("Balance");
                builder.Property(x => x.CreditLine).HasColumnName("CreditLine");
                builder.Property(x => x.Tax).HasColumnName("Tax");
                builder.Property(x => x.CreateBy).HasColumnName("CreateBy");
                builder.Property(x => x.CreateDate).HasColumnName("CreateDate");
                builder.Property(x => x.UpdateBy).HasColumnName("UpdateBy");
                builder.Property(x => x.UpdateDate).HasColumnName("UpdateDate");
                builder.Property(x => x.Active).HasColumnName("Active");
                builder.ToTable("Supplier");
            }
        }
    }
}
