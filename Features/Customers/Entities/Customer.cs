using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Pos.WebApi.Features.Customers.Entities
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }
        public string Rtn { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public int SellerId { get; set; }
        public int CustomerCategoryId { get; set; }
        public int PayConditionId { get; set; }
        public int ListPriceId { get; set; }
        public decimal Balance { get; set; }
        public decimal CreditLine { get; set; }
        public bool Tax { get; set; }
        public int CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public int UpdateBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool Active { get; set; }
        public int LimitInvoiceCredit { get; set; }
        public int TotalInvoiceCredit { get; set; }
        public string ContactPerson { get; set; }
        public bool Purchase { get; set; }
        public int FrequencyId { get; set; }
        public int ZoneId { get; set; }
        public int RegionId { get; set; }

        public bool IsValid()
        {
            if (string.IsNullOrEmpty(this.CustomerName)) throw new System.Exception("Debe ingresar un nombre.");
            if (string.IsNullOrEmpty(this.Rtn)) throw new System.Exception("Debe ingresar el RTN.");
            if ((this.CustomerCategoryId) == 0) throw new System.Exception("Debe seleccionar una categoria.");
            if ((this.PayConditionId) == 0) throw new System.Exception("Debe seleccionar una condicion de pago.");
            if ((this.SellerId) == 0) throw new System.Exception("Debe seleccionar un vendedor para este cliente.");
            if ((this.ListPriceId) == 0) throw new System.Exception("Debe seleccionar una lista de precio para este cliente.");
            if ((this.PayConditionId) != 1 && this.CreditLine==0) throw new System.Exception("Debe ingresar el limite de credito.");
            return true;
        }

        public class Map
        {
            public Map(EntityTypeBuilder<Customer> builder)
            {
                builder.HasKey(x => x.CustomerId);
                builder.Property(x => x.CustomerId).HasColumnName("CustomerId");
                builder.Property(x => x.CustomerName).HasColumnName("CustomerName");
                builder.Property(x => x.CustomerCode).HasColumnName("CustomerCode");
                builder.Property(x => x.Rtn).HasColumnName("Rtn");
                builder.Property(x => x.Phone).HasColumnName("Phone");
                builder.Property(x => x.Email).HasColumnName("Email");
                builder.Property(x => x.Address).HasColumnName("Address");
                builder.Property(x => x.SellerId).HasColumnName("SellerId");
                builder.Property(x => x.CustomerCategoryId).HasColumnName("CustomerCategoryId");
                builder.Property(x => x.PayConditionId).HasColumnName("PayConditionId");
                builder.Property(x => x.ListPriceId).HasColumnName("ListPriceId");
                builder.Property(x => x.Balance).HasColumnName("Balance");
                builder.Property(x => x.CreditLine).HasColumnName("CreditLine");
                builder.Property(x => x.Tax).HasColumnName("Tax");
                builder.Property(x => x.CreateBy).HasColumnName("CreateBy");
                builder.Property(x => x.CreateDate).HasColumnName("CreateDate");
                builder.Property(x => x.UpdateBy).HasColumnName("UpdateBy");
                builder.Property(x => x.UpdateDate).HasColumnName("UpdateDate");
                builder.Property(x => x.Active).HasColumnName("Active");
                builder.Property(x => x.LimitInvoiceCredit).HasColumnName("LimitInvoiceCredit");
                builder.Property(x => x.TotalInvoiceCredit).HasColumnName("TotalInvoiceCredit");
                builder.Property(x => x.ContactPerson).HasColumnName("ContactPerson");
                builder.Property(x => x.Purchase).HasColumnName("Purchase");
                builder.Property(x => x.FrequencyId).HasColumnName("FrequencyId");
                builder.Property(x => x.ZoneId).HasColumnName("ZoneId");
                builder.Property(x => x.RegionId).HasColumnName("RegionId");
                builder.ToTable("Customer");
            }
        }
    }
}
