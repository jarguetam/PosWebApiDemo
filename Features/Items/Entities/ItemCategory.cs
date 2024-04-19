using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pos.WebApi.Features.Items.Entities;
using System;

namespace Pos.WebApi.Features.Items.Entities
{
    public class ItemCategory
    {
        public int ItemCategoryId { get; set; }
        public string ItemCategoryName { get; set; }
        public int CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public int UpdateBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool Active { get; set; }

        public bool IsValid()
        {
            if (string.IsNullOrEmpty(this.ItemCategoryName)) throw new System.Exception("Debe ingresar un nombre");
            return true;
        }

        public class Map
        {
            public Map(EntityTypeBuilder<ItemCategory> builder)
            {
                builder.HasKey(x => x.ItemCategoryId);
                builder.Property(x => x.ItemCategoryId).HasColumnName("ItemCategoryId");
                builder.Property(x => x.ItemCategoryName).HasColumnName("ItemCategoryName");
                builder.Property(x => x.CreateBy).HasColumnName("CreateBy");
                builder.Property(x => x.CreateDate).HasColumnName("CreateDate");
                builder.Property(x => x.UpdateBy).HasColumnName("UpdateBy");
                builder.Property(x => x.UpdateDate).HasColumnName("UpdateDate");
                builder.Property(x => x.Active).HasColumnName("Active");
                builder.ToTable("ItemCategory");
            }
        }
    }
}
