using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Pos.WebApi.Features.InventoryTransactions.Entities
{
    public class CostRevaluation
    {
        public int Id { get; set; }
        public int ItemId { get; set; }      
        public decimal PreviousCost { get; set; }
        public decimal NewCost { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreateBy { get; set; }       
        public string Comment { get; set; }
        public int WhsCode { get; set; }

        public bool IsValid()
        {
            if ((this.WhsCode) == 0) throw new System.Exception("Debe venir el almacen.");
            if ((this.ItemId) == 0) throw new System.Exception("Debe seleccionar un producto.");
            if (string.IsNullOrEmpty(this.Comment)) throw new System.Exception("Debe agregar un comentario.");
            return true;
        }

        public class Map
        {
            public Map(EntityTypeBuilder<CostRevaluation> builder)
            {
                builder.HasKey(x => x.Id);
                builder.Property(x => x.Id).HasColumnName("Id");
                builder.Property(x => x.ItemId).HasColumnName("ItemId");
                builder.Property(x => x.PreviousCost).HasColumnName("PreviousCost");
                builder.Property(x => x.NewCost).HasColumnName("NewCost");
                builder.Property(x => x.CreateDate).HasColumnName("CreateDate");
                builder.Property(x => x.CreateBy).HasColumnName("CreateBy");
                builder.Property(x => x.Comment).HasColumnName("Comment");
                builder.Property(x => x.WhsCode).HasColumnName("WhsCode");
                builder.ToTable("CostRevaluation");
            }
        }
    }
}
