using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Pos.WebApi.Features.Items.Entities
{
    public class Item
    {
        public int ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public int ItemCategoryId { get; set; }
        public int ItemFamilyId { get; set; }
        public int UnitOfMeasureId { get; set; }
        public decimal Stock { get; set; }
        public decimal AvgPrice { get; set; }
        public bool SalesItem { get; set; }
        public bool PurchaseItem { get; set; }
        public bool InventoryItem { get; set; }
        public bool Tax { get; set; }
        public decimal Weight { get; set; }
        public decimal PricePurchase { get; set; }
        public string BarCode { get; set; }
        public int CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public int UpdateBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool Active { get; set; }
       

        public bool IsValid()
        {
            if (string.IsNullOrEmpty(this.ItemName)) throw new System.Exception("Debe ingresar un nombre");
            if (this.ItemCategoryId==0) throw new System.Exception("Debe seleccionar la categoria");
            if (this.UnitOfMeasureId==0) throw new System.Exception("Debe seleccionar una unidad de medida");
            if (string.IsNullOrEmpty(this.ItemName)) throw new System.Exception("Debe ingresar un nombre");
            if (string.IsNullOrEmpty(this.ItemName)) throw new System.Exception("Debe ingresar un nombre");
            if (this.ItemFamilyId == 0) throw new System.Exception("Debe seleccionar la sub categoria");
            return true;
        }

        public class Map
        {
            public Map(EntityTypeBuilder<Item> builder)
            {
                builder.HasKey(x => x.ItemId);
                builder.Property(x => x.ItemId).HasColumnName("ItemId");
                builder.Property(x => x.ItemCode).HasColumnName("ItemCode");
                builder.Property(x => x.ItemName).HasColumnName("ItemName");
                builder.Property(x => x.ItemCategoryId).HasColumnName("ItemCategoryId");
                builder.Property(x => x.ItemFamilyId).HasColumnName("ItemFamilyId");
                builder.Property(x => x.UnitOfMeasureId).HasColumnName("UnitOfMeasureId");
                builder.Property(x => x.Stock).HasColumnName("Stock");
                builder.Property(x => x.AvgPrice).HasColumnName("AvgPrice");
                builder.Property(x => x.SalesItem).HasColumnName("SalesItem");
                builder.Property(x => x.PurchaseItem).HasColumnName("PurchaseItem");
                builder.Property(x => x.InventoryItem).HasColumnName("InventoryItem");
                builder.Property(x => x.Tax).HasColumnName("Tax");
                builder.Property(x => x.Weight).HasColumnName("Weight");
                builder.Property(x => x.BarCode).HasColumnName("BarCode");
                builder.Property(x => x.PricePurchase).HasColumnName("PricePurchase");
                builder.Property(x => x.CreateBy).HasColumnName("CreateBy");
                builder.Property(x => x.CreateDate).HasColumnName("CreateDate");
                builder.Property(x => x.UpdateBy).HasColumnName("UpdateBy");
                builder.Property(x => x.UpdateDate).HasColumnName("UpdateDate");
                builder.Property(x => x.Active).HasColumnName("Active");
                builder.ToTable("Item");
            }
        }
    }
}
