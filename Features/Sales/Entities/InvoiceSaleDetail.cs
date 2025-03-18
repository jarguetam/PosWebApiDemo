using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pos.WebApi.Features.Items.Entities;
using System;
using System.Text.Json.Serialization;

namespace Pos.WebApi.Features.Sales.Entities
{
    public class InvoiceSaleDetail
    {
        public int DocDetailId { get; set; }
        public int DocId { get; set; }
        public int ItemId { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Cost { get; set; }
        public DateTime DueDate { get; set; }
        public decimal LineTotal { get; set; }
        public int WhsCode { get; set; }
        public int UnitOfMeasureId { get; set; }
        public bool IsDelete { get; set; }
        public decimal Weight { get; set; }
        [JsonIgnore]
        public InvoiceSale InvoiceSale { get; set; }
        [JsonIgnore]
        public Item Items { get; set; }
        [JsonIgnore]
        public WareHouse WareHouse { get; set; }
        [JsonIgnore]
        public UnitOfMeasure UnitOfMeasure { get; set; }
        public bool IsValid()
        {
            if ((this.WhsCode) == 0) throw new System.Exception("Debe venir el almacen.");
            if ((this.Quantity) == 0) throw new System.Exception("La cantidad debe ser mayor a 0.");
            if ((this.Price) == 0) throw new System.Exception("Debe agregar el precio.");
            if ((this.Cost) == 0) throw new System.Exception("Este articulo no tiene costo. Contacte a su administrador.");
            return true;
        }

        public class Map
        {
            public Map(EntityTypeBuilder<InvoiceSaleDetail> builder)
            {
                builder.HasKey(x => x.DocDetailId);
                builder.Property(x => x.DocDetailId).HasColumnName("DocDetailId");
                builder.Property(x => x.DocId).HasColumnName("DocId");
                builder.Property(x => x.WhsCode).HasColumnName("WhsCode");
                builder.Property(x => x.ItemId).HasColumnName("ItemId");
                builder.Property(x => x.Quantity).HasColumnName("Quantity");
                builder.Property(x => x.UnitOfMeasureId).HasColumnName("UnitOfMeasureId");
                builder.Property(x => x.Price).HasColumnName("Price");
                builder.Property(x => x.Cost).HasColumnName("Cost");
                builder.Property(x => x.DueDate).HasColumnName("DueDate");
                builder.Property(x => x.LineTotal).HasColumnName("LineTotal");
                builder.Property(x => x.IsDelete).HasColumnName("IsDelete");
                builder.Property(x => x.Weight).HasColumnName("Weight");
                builder.HasOne(x => x.InvoiceSale).WithMany(x => x.Detail).HasForeignKey(x => x.DocId);
                builder.HasOne(x => x.Items)
                      .WithMany()
                      .HasForeignKey(x => x.ItemId);
                builder.HasOne(x => x.WareHouse)
                      .WithMany()
                      .HasForeignKey(x => x.WhsCode);
                builder.HasOne(x => x.UnitOfMeasure)
                      .WithMany()
                      .HasForeignKey(x => x.UnitOfMeasureId);
                builder.ToTable("InvoiceSaleDetail");
            }
        }
    }
}
