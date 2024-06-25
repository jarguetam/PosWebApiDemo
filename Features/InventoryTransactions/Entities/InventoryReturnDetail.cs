using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json.Serialization;

namespace Pos.WebApi.Features.InventoryTransactions.Entities
{
    public class InventoryReturnDetail
    {
        public int IdDetail { get; set; }
        public int IdReturn { get; set; }
        public int ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal QuantityInitial { get; set; }
        public decimal QuantityWareHouse { get; set; }
        public decimal QuantitySeller { get; set; }
        public decimal QuantityOutPut { get; set; }
        public decimal QuantityReturn { get; set; }
        public decimal QuantityDiference { get; set; }
        public string  Comment { get;set; }
        [JsonIgnore]
        public InventoryReturn InventoryReturn { get; set; }


        public class Map
        {
            public Map(EntityTypeBuilder<InventoryReturnDetail> builder)
            {
                builder.HasKey(x => x.IdDetail);
                builder.Property(x => x.IdReturn).HasColumnName("IdReturn");
                builder.Property(x => x.ItemId).HasColumnName("ItemId");
                builder.Property(x => x.ItemCode).HasColumnName("ItemCode");
                builder.Property(x => x.ItemName).HasColumnName("ItemName");
                builder.Property(x => x.QuantityInitial).HasColumnName("QuantityInitial");
                builder.Property(x => x.QuantityWareHouse).HasColumnName("QuantityWareHouse");
                builder.Property(x => x.QuantitySeller).HasColumnName("QuantitySeller");
                builder.Property(x => x.QuantityReturn).HasColumnName("QuantityReturn");
                builder.Property(x => x.QuantityOutPut).HasColumnName("QuantityOutPut");
                builder.Property(x => x.QuantityDiference).HasColumnName("QuantityDiference");
                builder.Property(x => x.Comment).HasColumnName("Comment");
                builder.HasOne(x => x.InventoryReturn).WithMany(x => x.Detail).HasForeignKey(x => x.IdReturn);
                builder.ToTable("InventoryReturnDetail");
            }
        }

    }
}
