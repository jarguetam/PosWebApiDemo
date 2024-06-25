using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pos.WebApi.Features.InventoryTransactions.Entities
{
    public class InventoryEntry
    {
        public int EntryId { get; set; }
        public DateTime EntryDate { get; set; }
        public string Comment { get; set; }
        public decimal DocTotal { get; set; }
        public int WhsCode { get; set; }
        public int Type { get; set; }
        public int CreateBy { get; set; }
        public DateTime CreateDate{ get; set; }
        public List<InventoryEntryDetail> Detail { get; set; }
        public InventoryEntry()
        {
            Detail = new List<InventoryEntryDetail>();
        }
        public bool IsValid()
        {
            if ((this.WhsCode) == 0) throw new System.Exception("Debe venir el almacen.");
            if ((this.DocTotal) == 0) throw new System.Exception("El total del documento debe ser mayor a 0.");
            if (string.IsNullOrEmpty(this.Comment)) throw new System.Exception("Debe agregar un comentario.");
            var existQtyWithZero = Detail.Where(x => x.Quantity == 0).Count();
            if (existQtyWithZero > 0) throw new Exception("Existen articulos con cantidad 0");
            var existPriceWithZero = Detail.Where(x => x.Price == 0).Count();
            if (existPriceWithZero > 0) throw new Exception("Existen articulos sin precio");
            var duedate = Detail.Where(x => x.DueDate.Date <= DateTime.Now.Date).Count();
            if (duedate > 0) throw new Exception("Fecha de vencimiento debe ser mayor a la fecha actual."); 
            return true;
        }

        public class Map
        {
            public Map(EntityTypeBuilder<InventoryEntry> builder)
            {
                builder.HasKey(x => x.EntryId);
                builder.Property(x => x.EntryId).HasColumnName("EntryId");
                builder.Property(x => x.EntryDate).HasColumnName("EntryDate");
                builder.Property(x => x.WhsCode).HasColumnName("WhsCode");
                builder.Property(x => x.Type).HasColumnName("Type");
                builder.Property(x => x.Comment).HasColumnName("Comment");
                builder.Property(x => x.CreateBy).HasColumnName("CreateBy");
                builder.Property(x => x.CreateDate).HasColumnName("CreatedDate");
                builder.HasMany(x => x.Detail).WithOne(x => x.InventoryEntry).HasForeignKey(x => x.EntryId);
                builder.ToTable("InventoryEntry");
            }
        }
    }
}
