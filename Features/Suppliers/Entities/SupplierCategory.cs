using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pos.WebApi.Features.Suppliers.Entities;
using System;

namespace Pos.WebApi.Features.Suppliers.Entities
{
    public class SupplierCategory
    {
        public int SupplierCategoryId { get; set; }
        public string SupplierCategoryName { get; set; }
        public int CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public int UpdateBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool Active { get; set; }

        public bool IsValid()
        {
            if (string.IsNullOrEmpty(this.SupplierCategoryName)) throw new System.Exception("Debe ingresar un nombre");
            return true;
        }

        public class Map
        {
            public Map(EntityTypeBuilder<SupplierCategory> builder)
            {
                builder.HasKey(x => x.SupplierCategoryId);
                builder.Property(x => x.SupplierCategoryId).HasColumnName("SupplierCategoryId");
                builder.Property(x => x.SupplierCategoryName).HasColumnName("SupplierCategoryName");
                builder.Property(x => x.CreateBy).HasColumnName("CreateBy");
                builder.Property(x => x.CreateDate).HasColumnName("CreateDate");
                builder.Property(x => x.UpdateBy).HasColumnName("UpdateBy");
                builder.Property(x => x.UpdateDate).HasColumnName("UpdateDate");
                builder.Property(x => x.Active).HasColumnName("Active");
                builder.ToTable("SupplierCategory");
            }
        }
    }
}
