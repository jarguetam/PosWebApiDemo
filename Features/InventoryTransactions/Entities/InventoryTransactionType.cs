using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Globalization;

namespace Pos.WebApi.Features.InventoryTransactions.Entities
{
    public class InventoryTransactionType
    {
        public int Id   { get; set; }
        public string Name { get; set; }
        public string Transaction { get; set; }
        public DateTime CreateDate { get; set; }
        public int CreateBy { get; set; }

        public bool IsValid()
        {
            if (string.IsNullOrEmpty(this.Name)) throw new System.Exception("Debe ingresar un nombre");
            if (string.IsNullOrEmpty(this.Transaction)) throw new System.Exception("Debe seleccionar el documento");
            if (this.Transaction =="N") throw new System.Exception("Debe seleccionar el documento");
            return true;
        }

        public class Map
        {
            public Map(EntityTypeBuilder<InventoryTransactionType> builder)
            {
                builder.HasKey(x => x.Id);
                builder.Property(x => x.Id).HasColumnName("Id");
                builder.Property(x => x.Name).HasColumnName("Name");
                builder.Property(x => x.Transaction).HasColumnName("Transaction");
                builder.Property(x => x.CreateBy).HasColumnName("CreateBy");
                builder.Property(x => x.CreateDate).HasColumnName("CreateDate");
                builder.ToTable("InventoryTransactionType");
            }
        }
    }
}
