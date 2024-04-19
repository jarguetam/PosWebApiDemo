using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Text.Json.Serialization;

namespace Pos.WebApi.Features.InventoryTransactions.Entities
{
    public class InventoryTransferDetail
    {
        public int TransferDetailId { get; set; }
        public int TransferId { get; set; }
        public int ItemId { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public DateTime DueDate { get; set; }
        public decimal LineTotal { get; set; }
        public int FromWhsCode { get; set; }
        public int ToWhsCode { get; set; }
        public int UnitOfMeasureId { get; set; }
        [JsonIgnore]
        public InventoryTransfer InventoryTransfer { get; set; }
        public bool IsValid()
        {
            if ((this.FromWhsCode) == 0) throw new System.Exception("Debe venir el almacen origen.");
            if ((this.ToWhsCode) == 0) throw new System.Exception("Debe venir el almacen destino.");
            if ((this.Quantity) == 0) throw new System.Exception("La cantidad debe ser mayor a 0.");
            if ((this.Price) == 0) throw new System.Exception("Debe agregar el precio.");
            return true;
        }

        public class Map
        {
            public Map(EntityTypeBuilder<InventoryTransferDetail> builder)
            {
                builder.HasKey(x => x.TransferDetailId);
                builder.Property(x => x.TransferDetailId).HasColumnName("TransferDetailId");
                builder.Property(x => x.TransferId).HasColumnName("TransferId");
                builder.Property(x => x.FromWhsCode).HasColumnName("FromWhsCode");
                builder.Property(x => x.ToWhsCode).HasColumnName("ToWhsCode");
                builder.Property(x => x.ItemId).HasColumnName("ItemId");
                builder.Property(x => x.UnitOfMeasureId).HasColumnName("UnitOfMeasureId");
                builder.Property(x => x.Quantity).HasColumnName("Quantity");
                builder.Property(x => x.Price).HasColumnName("Price");
                builder.Property(x => x.DueDate).HasColumnName("DueDate");
                builder.Property(x => x.LineTotal).HasColumnName("LineTotal");
                builder.HasOne(x => x.InventoryTransfer).WithMany(x => x.Detail).HasForeignKey(x => x.TransferId);
                builder.ToTable("InventoryTransferDetail");
            }
        }
    }
}
