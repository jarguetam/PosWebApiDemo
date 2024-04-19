using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Pos.WebApi.Features.Common.Entities
{
    public class SarPointSale
    {
        public string PointSaleId { get; set; }
        public string Name { get; set; }
        public string BranchId { get; set; }
        
        public bool Active { get; set; }
        public int UserId { get; set; }
        public DateTime CreateDate { get; set; }

        
        public class Map
        {
            public Map(EntityTypeBuilder<SarPointSale> builder)
            {
                builder.HasKey(x => new { x.PointSaleId, x.BranchId });
                builder.Property(x => x.PointSaleId).HasColumnName("PointSaleId");
                builder.Property(x => x.Name).HasColumnName("Name");
                builder.Property(x => x.BranchId).HasColumnName("BranchId");
                builder.Property(x => x.Active).HasColumnName("Active");
                builder.Property(x => x.UserId).HasColumnName("UserId");
                builder.Property(x => x.CreateDate).HasColumnName("CreateDate");
                builder.ToTable("SarPointSale");
            }
        }
    }
}
