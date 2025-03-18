using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Text.Json.Serialization;

namespace Pos.WebApi.Features.InventoryTransactions.Entities
{
    public class InventoryRequestTransferDetail
    {
        public int TransferRequestDetailId { get; set; }
        public int TransferRequestId { get; set; }
        public int ItemId { get; set; }
        public decimal Quantity { get; set; }
        public string QuantityUnit { get; set; }
        public decimal Price { get; set; }
        public DateTime DueDate { get; set; }
        public decimal LineTotal { get; set; }
        public int FromWhsCode { get; set; }
        public int ToWhsCode { get; set; }
        public int UnitOfMeasureId { get; set; }
        [JsonIgnore]
        public InventoryRequestTransfer InventoryRequestTransfer { get; set; }
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
            public Map(EntityTypeBuilder<InventoryRequestTransferDetail> builder)
            {
                builder.HasKey(x => x.TransferRequestDetailId);
                builder.Property(x => x.TransferRequestDetailId).HasColumnName("TransferRequestDetailId");
                builder.Property(x => x.TransferRequestId).HasColumnName("TransferRequestId");
                builder.Property(x => x.FromWhsCode).HasColumnName("FromWhsCode");
                builder.Property(x => x.ToWhsCode).HasColumnName("ToWhsCode");
                builder.Property(x => x.ItemId).HasColumnName("ItemId");
                builder.Property(x => x.UnitOfMeasureId).HasColumnName("UnitOfMeasureId");
                builder.Property(x => x.Quantity).HasColumnName("Quantity");
                builder.Property(x => x.QuantityUnit).HasColumnName("QuantityUnit");
                builder.Property(x => x.Price).HasColumnName("Price");
                builder.Property(x => x.DueDate).HasColumnName("DueDate");
                builder.Property(x => x.LineTotal).HasColumnName("LineTotal");
                builder.HasOne(x => x.InventoryRequestTransfer).WithMany(x => x.Detail).HasForeignKey(x => x.TransferRequestId);
                builder.ToTable("InventoryRequestTransferDetail");
            }
        }
    }
}
