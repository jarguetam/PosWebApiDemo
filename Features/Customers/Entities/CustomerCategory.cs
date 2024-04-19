using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Pos.WebApi.Features.Customers.Entities
{
    public class CustomerCategory
    {
        public int CustomerCategoryId { get; set; }
        public string CustomerCategoryName { get; set; }
        public int CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public int UpdateBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool Active { get; set; }

        public bool IsValid()
        {
            if (string.IsNullOrEmpty(this.CustomerCategoryName)) throw new System.Exception("Debe ingresar un nombre");
            return true;
        }

        public class Map
        {
            public Map(EntityTypeBuilder<CustomerCategory> builder)
            {
                builder.HasKey(x => x.CustomerCategoryId);
                builder.Property(x => x.CustomerCategoryId).HasColumnName("CustomerCategoryId");
                builder.Property(x => x.CustomerCategoryName).HasColumnName("CustomerCategoryName");
                builder.Property(x => x.CreateBy).HasColumnName("CreateBy");
                builder.Property(x => x.CreateDate).HasColumnName("CreateDate");
                builder.Property(x => x.UpdateBy).HasColumnName("UpdateBy");
                builder.Property(x => x.UpdateDate).HasColumnName("UpdateDate");
                builder.Property(x => x.Active).HasColumnName("Active");
                builder.ToTable("CustomerCategory");
            }
        }
    }
}
