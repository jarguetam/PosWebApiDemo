using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pos.WebApi.Features.Purchase.Entities;
using System.Collections.Generic;
using System;
using Microsoft.EntityFrameworkCore;

namespace Pos.WebApi.Features.PurchasePayment.Entitie
{
    public class PaymentPurchase
    {
        public int DocId { get; set; }
        public int SupplierId { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public int PayConditionId { get; set; }
        public DateTime DocDate { get; set; }
        public bool Canceled { get; set; }
        public string Comment { get; set; }
        public string Reference { get; set; }
        public decimal CashSum { get; set; }
        public decimal ChekSum { get; set; }
        public decimal TransferSum { get; set; }
        public decimal CardSum { get; set; }
        public decimal DocTotal { get; set; }
        public int CreateBy { get; set; }
        public bool Complete { get; set; }
        public List<PaymentPurchaseDetail> Detail { get; set; }
        public PaymentPurchase()
        {
            Detail = new List<PaymentPurchaseDetail>();
        }
        public bool IsValid()
        {
            if ((this.DocTotal) == 0) throw new System.Exception("El total del documento debe ser mayor a 0.");
            if(Math.Round((this.CashSum+ this.TransferSum+ this.ChekSum + this.CardSum),2)==0) throw new System.Exception("Debe ingresar al menos un medio de pago.");
            if (Math.Round((this.CashSum + this.TransferSum + this.ChekSum + this.CardSum),2) < Math.Round(this.DocTotal,2)) throw new System.Exception("Debe completar la cantidad a pagar.");
            if (string.IsNullOrEmpty(this.Comment)) throw new System.Exception("Debe agregar un comentario.");       
            return true;
        }

        public class Map
        {
            public Map(EntityTypeBuilder<PaymentPurchase> builder)
            {
                builder.HasKey(x => x.DocId);
                builder.Property(x => x.DocId).HasColumnName("DocId");
                builder.Property(x => x.SupplierId).HasColumnName("SupplierId");
                builder.Property(x => x.SupplierCode).HasColumnName("SupplierCode");
                builder.Property(x => x.SupplierName).HasColumnName("SupplierName");
                builder.Property(x => x.PayConditionId).HasColumnName("PayConditionId");
                builder.Property(x => x.DocDate).HasColumnName("DocDate");
                builder.Property(x => x.Canceled).HasColumnName("Canceled");
                builder.Property(x => x.Comment).HasColumnName("Comment");
                builder.Property(x => x.Reference).HasColumnName("Reference");
                builder.Property(x => x.CashSum).HasColumnName("CashSum");
                builder.Property(x => x.ChekSum).HasColumnName("ChekSum");
                builder.Property(x => x.TransferSum).HasColumnName("TransferSum");
                builder.Property(x => x.CardSum).HasColumnName("CardSum");
                builder.Property(x => x.DocTotal).HasColumnName("DocTotal");
                builder.Property(x => x.Comment).HasColumnName("Comment");
                builder.Property(x => x.CreateBy).HasColumnName("CreateBy");
                builder.Property(x => x.Complete).HasColumnName("Complete");
                builder.HasMany(x => x.Detail).WithOne(x => x.PaymentPurchase).HasForeignKey(x => x.DocId);
                builder.ToTable("PaymentPurchase");
            }
        }

    }
}
