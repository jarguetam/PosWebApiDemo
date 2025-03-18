using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Pos.WebApi.Features.Common.Entities;
using Pos.WebApi.Features.Users.Entities;

namespace Pos.WebApi.Features.SalesPayment.Entities
{
    public class PaymentSale
    {
        public int DocId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
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
        public int SellerId { get; set; }
        public string Uuid { get; set; }
        public bool Offline { get; set; }
        public List<PaymentSaleDetail> Detail { get; set; }

        public User User { get; set; }
        public PayCondition PayCondition { get; set; }
        public PaymentSale()
        {
            Detail = new List<PaymentSaleDetail>();
        }
        public bool IsValid()
        {
            if ((this.DocTotal) == 0) throw new System.Exception("El total del documento debe ser mayor a 0.");
            if ((this.CashSum + this.TransferSum + this.ChekSum + this.CardSum) == 0) throw new System.Exception("Debe ingresar al menos un medio de pago.");
            if (Math.Round((this.CashSum + this.TransferSum + this.ChekSum + this.CardSum),2) < Math.Round(this.DocTotal, 2) ) throw new System.Exception("Debe completar la cantidad a pagar.");
            if (string.IsNullOrEmpty(this.Comment)) throw new System.Exception("Debe agregar un comentario.");
            return true;
        }

        public class Map
        {
            public Map(EntityTypeBuilder<PaymentSale> builder)
            {
                builder.HasKey(x => x.DocId);
                builder.Property(x => x.DocId).HasColumnName("DocId");
                builder.Property(x => x.CustomerId).HasColumnName("CustomerId");
                builder.Property(x => x.CustomerCode).HasColumnName("CustomerCode");
                builder.Property(x => x.CustomerName).HasColumnName("CustomerName");
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
                builder.Property(x => x.SellerId).HasColumnName("SellerId");
                builder.Property(x => x.Uuid).HasColumnName("Uuid");
                builder.Property(x => x.Offline).HasColumnName("Offline");
                builder.HasMany(x => x.Detail).WithOne(x => x.PaymentSale).HasForeignKey(x => x.DocId);
       
                builder.HasOne(x => x.User)
                      .WithMany()
                      .HasForeignKey(x => x.CreateBy);

                builder.HasOne(x => x.PayCondition)
                      .WithMany()
                      .HasForeignKey(x => x.PayConditionId);
                builder.ToTable("PaymentSale");
            }
        }
    }
}
