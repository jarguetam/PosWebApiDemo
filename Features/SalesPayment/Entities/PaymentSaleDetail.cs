using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Text.Json.Serialization;

namespace Pos.WebApi.Features.SalesPayment.Entities
{
    public class PaymentSaleDetail
    {
        public int DocDetailId { get; set; }
        public int DocId { get; set; }
        public int InvoiceId { get; set; }
        public string InvoiceReference { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxTotal { get; set; }
        public decimal DiscountTotal { get; set; }
        public decimal LineTotal { get; set; }

        public decimal SumApplied { get; set; }
        [JsonIgnore]
        public PaymentSale PaymentSale { get; set; }
        public bool IsValid()
        {
            if ((this.InvoiceId) == 0) throw new System.Exception("Debe venir el numero de factura.");

            return true;
        }

        public class Map
        {
            public Map(EntityTypeBuilder<PaymentSaleDetail> builder)
            {
                builder.HasKey(x => x.DocDetailId);
                builder.Property(x => x.DocDetailId).HasColumnName("DocDetailId");
                builder.Property(x => x.DocId).HasColumnName("DocId");
                builder.Property(x => x.InvoiceId).HasColumnName("InvoiceId");
                builder.Property(x => x.InvoiceReference).HasColumnName("InvoiceReference");
                builder.Property(x => x.InvoiceDate).HasColumnName("InvoiceDate");
                builder.Property(x => x.DueDate).HasColumnName("DueDate");
                builder.Property(x => x.SubTotal).HasColumnName("SubTotal");
                builder.Property(x => x.TaxTotal).HasColumnName("TaxTotal");
                builder.Property(x => x.DiscountTotal).HasColumnName("DiscountTotal");
                builder.Property(x => x.LineTotal).HasColumnName("LineTotal");
                builder.Property(x => x.SumApplied).HasColumnName("SumApplied");
                builder.HasOne(x => x.PaymentSale).WithMany(x => x.Detail).HasForeignKey(x => x.DocId);
                builder.ToTable("PaymentSaleDetail");
            }
        }
    }
}
