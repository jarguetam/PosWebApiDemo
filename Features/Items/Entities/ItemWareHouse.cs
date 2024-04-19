using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Pos.WebApi.Features.Items.Entities
{
    public class ItemWareHouse
    {
        public int ItemWareHouseId { get; set; }
        public int ItemId { get; set; }
        public int WhsCode { get; set; }
        public decimal Stock { get; set; }
        public decimal AvgPrice { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime DueDate { get; set; }

        public bool IsValid()
        {
            if (ItemId==0) throw new System.Exception("Debe venir el articulo");
            if (WhsCode == 0) throw new System.Exception("Debe venir el almacen");
            return true;
        }

        public class Map
        {
            public Map(EntityTypeBuilder<ItemWareHouse> builder)
            {
                builder.HasKey(x => x.ItemWareHouseId);
                builder.Property(x => x.ItemWareHouseId).HasColumnName("ItemWareHouseId");
                builder.Property(x => x.ItemId).HasColumnName("ItemId");
                builder.Property(x => x.WhsCode).HasColumnName("WhsCode");
                builder.Property(x => x.Stock).HasColumnName("Stock");
                builder.Property(x => x.AvgPrice).HasColumnName("AvgPrice");
                builder.Property(x => x.CreateDate).HasColumnName("CreateDate");
                builder.Property(x => x.DueDate).HasColumnName("DueDate");
                builder.ToTable("ItemWareHouse");
            }
        }
    }
}
