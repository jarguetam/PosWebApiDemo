using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pos.WebApi.Features.Purchase.Dto;
using System;
using System.Text.Json.Serialization;

namespace Pos.WebApi.Features.Purchase.Entities
{
    public class OrderPurchaseDetail
    {
        public int DocDetailId { get; set; }
        public int DocId { get; set; }
        public int ItemId { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public DateTime DueDate { get; set; }
        public decimal LineTotal { get; set; }
        public decimal TaxValue { get; set; }
        public int WhsCode { get; set; }
        public int UnitOfMeasureId { get; set; }
        public bool IsDelete { get; set; }
        [JsonIgnore]
        public OrderPurchase OrderPurchase { get; set; }
        public bool IsValid()
        {
            if ((this.WhsCode) == 0) throw new System.Exception("Debe venir el almacen.");
            if ((this.Quantity) == 0) throw new System.Exception("La cantidad debe ser mayor a 0.");
            if ((this.Price) == 0) throw new System.Exception("Debe agregar el precio.");
            return true;
        }

        public class Map
        {
            public Map(EntityTypeBuilder<OrderPurchaseDetail> builder)
            {
                builder.HasKey(x => x.DocDetailId);
                builder.Property(x => x.DocDetailId).HasColumnName("DocDetailId");
                builder.Property(x => x.DocId).HasColumnName("DocId");
                builder.Property(x => x.WhsCode).HasColumnName("WhsCode");
                builder.Property(x => x.ItemId).HasColumnName("ItemId");
                builder.Property(x => x.Quantity).HasColumnName("Quantity");
                builder.Property(x => x.UnitOfMeasureId).HasColumnName("UnitOfMeasureId");
                builder.Property(x => x.Price).HasColumnName("Price");
                builder.Property(x => x.DueDate).HasColumnName("DueDate");
                builder.Property(x => x.LineTotal).HasColumnName("LineTotal");
                builder.Property(x => x.TaxValue).HasColumnName("TaxValue");
                builder.Property(x => x.IsDelete).HasColumnName("IsDelete");
                builder.HasOne(x => x.OrderPurchase).WithMany(x => x.Detail).HasForeignKey(x => x.DocId);
                builder.ToTable("OrderPurchaseDetail");
            }
        }

    }
}
