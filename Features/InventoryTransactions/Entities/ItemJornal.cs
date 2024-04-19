using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pos.WebApi.Features.Suppliers.Entities;
using System;

namespace Pos.WebApi.Features.InventoryTransactions.Entities
{
    public class ItemJournal
    {
        public long ItemJournalId { get; set; }
        public int ItemId { get; set; }
        public int WhsCode { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TransValue { get; set; }
        public string Documents { get; set; }
        public int DocumentReferent { get; set; }
        public int CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
      
        public bool IsValid()
        {
            if ((this.ItemId) == 0) throw new System.Exception("Debe venir el articulo. Contactese con su administrador.");
            if ((this.WhsCode) == 0) throw new System.Exception("Debe venir el almacen. Contactese con su administrador.");
            if ((this.Quantity)== 0) throw new System.Exception("Cantidad debe ser mayor a 0. Contactese con su administrador.");
            if ((this.Price) == 0) throw new System.Exception("La transaccion debe tener un valor. Contactese con su administrador.");
            return true;
        }

        public class Map
        {
            public Map(EntityTypeBuilder<ItemJournal> builder)
            {
                builder.HasKey(x => x.ItemJournalId);
                builder.Property(x => x.ItemJournalId).HasColumnName("ItemJournalId");
                builder.Property(x => x.ItemId).HasColumnName("ItemId");
                builder.Property(x => x.WhsCode).HasColumnName("WhsCode");
                builder.Property(x => x.Quantity).HasColumnName("Quantity");
                builder.Property(x => x.Price).HasColumnName("Price");
                builder.Property(x => x.TransValue).HasColumnName("TransValue");
                builder.Property(x => x.Documents).HasColumnName("Documents");
                builder.Property(x => x.DocumentReferent).HasColumnName("DocumentReferent");
                builder.Property(x => x.CreateBy).HasColumnName("CreateBy");
                builder.Property(x => x.CreateDate).HasColumnName("CreateDate");
                builder.ToTable("ItemJournal");
            }
        }
    }
}
