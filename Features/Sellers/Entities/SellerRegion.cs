using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Pos.WebApi.Features.Sellers.Entities
{
    public class SellerRegion
    {
        public int RegionId { get; set; }
        public string NameRegion { get; set; }
        public int CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public int UpdateBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool Active { get; set; }

        public bool IsValid()
        {
            if (string.IsNullOrEmpty(this.NameRegion)) throw new System.Exception("Debe ingresar un nombre");
            return true;
        }

        public class Map
        {
            public Map(EntityTypeBuilder<SellerRegion> builder)
            {
                builder.HasKey(x => x.RegionId);
                builder.Property(x => x.RegionId).HasColumnName("RegionId");
                builder.Property(x => x.NameRegion).HasColumnName("NameRegion");
                builder.Property(x => x.CreateBy).HasColumnName("CreateBy");
                builder.Property(x => x.CreateDate).HasColumnName("CreateDate");
                builder.Property(x => x.UpdateBy).HasColumnName("UpdateBy");
                builder.Property(x => x.UpdateDate).HasColumnName("UpdateDate");
                builder.Property(x => x.Active).HasColumnName("Active");
                builder.ToTable("SellerRegion");
            }
        }
    }
}
