using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Pos.WebApi.Features.InventoryTransactions.Entities
{
    public class InventoryReturn
    {
        public int Id { get; set; }
        public DateTime DocDate { get; set; }
        public int SellerId { get; set; }
        public int RegionId { get; set; }
        public int WhsCode { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public bool Canceled { get; set; }
        public bool Active { get; set; }
        public bool Complete { get; set; }
        public List<InventoryReturnDetail> Detail { get; set; }
        public InventoryReturn()
        {
            Detail = new List<InventoryReturnDetail>();
        }


        public class Map
        {
            public Map(EntityTypeBuilder<InventoryReturn> builder)
            {
                builder.HasKey(x => x.Id);
                builder.Property(x => x.DocDate).HasColumnName("DocDate");
                builder.Property(x => x.SellerId).HasColumnName("SellerId");
                builder.Property(x => x.RegionId).HasColumnName("RegionId");
                builder.Property(x => x.WhsCode).HasColumnName("WhsCode");
                builder.Property(x => x.CreatedDate).HasColumnName("CreatedDate");
                builder.Property(x => x.CreatedBy).HasColumnName("CreatedBy");
                builder.Property(x => x.Canceled).HasColumnName("Canceled");
                builder.Property(x => x.Active).HasColumnName("Active");
                builder.Property(x => x.Complete).HasColumnName("Complete");
                builder.HasMany(x => x.Detail).WithOne(x => x.InventoryReturn).HasForeignKey(x => x.IdReturn);
                builder.ToTable("InventoryReturn");
            }
        }

    }
}
