using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Text.Json.Serialization;

namespace Pos.WebApi.Features.InventoryTransactions.Entities
{
    public class InventoryOutPutDetail
    {
        public int OutputDetailId { get; set; }
        public int OutputId { get; set; }
        public int ItemId { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public DateTime DueDate { get; set; }
        public decimal LineTotal { get; set; }
        public int WhsCode { get; set; }
        public int UnitOfMeasureId { get; set; }

        [JsonIgnore]
        public InventoryOutPut InventoryOutPut { get; set; }
        public bool IsValid()
        {
            if ((this.WhsCode) == 0) throw new System.Exception("Debe venir el almacen.");
            if ((this.Quantity) == 0) throw new System.Exception("La cantidad debe ser mayor a 0.");
            if ((this.Price) == 0) throw new System.Exception("Debe agregar el precio.");
            return true;
        }

        public class Map
        {
            public Map(EntityTypeBuilder<InventoryOutPutDetail> builder)
            {
                builder.HasKey(x => x.OutputDetailId);
                builder.Property(x => x.OutputDetailId).HasColumnName("OutputDetailId");
                builder.Property(x => x.OutputId).HasColumnName("OutputId");
                builder.Property(x => x.WhsCode).HasColumnName("WhsCode");
                builder.Property(x => x.ItemId).HasColumnName("ItemId");
                builder.Property(x => x.Quantity).HasColumnName("Quantity");
                builder.Property(x => x.UnitOfMeasureId).HasColumnName("UnitOfMeasureId");
                builder.Property(x => x.Price).HasColumnName("Price");
                builder.Property(x => x.DueDate).HasColumnName("DueDate");
                builder.Property(x => x.LineTotal).HasColumnName("LineTotal");
                builder.HasOne(x => x.InventoryOutPut).WithMany(x => x.Detail).HasForeignKey(x => x.OutputId);
                builder.ToTable("InventoryOutPutDetail");
            }
        }
    }
}
