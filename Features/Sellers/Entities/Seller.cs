using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Pos.WebApi.Features.Sellers.Entities
{
    public class Seller
    {
        public int SellerId { get; set; }
        public string SellerName { get; set; }
        public int WhsCode { get; set; }
        public int RegionId { get; set; }
        public int CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public int UpdateBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool Active { get; set; }

        public bool IsValid()
        {
            if (string.IsNullOrEmpty(this.SellerName)) throw new System.Exception("Debe ingresar un nombre");
            if (this.RegionId ==0) throw new System.Exception("Debe seleccionar la region");
            if (this.WhsCode == 0) throw new System.Exception("Debe seleccionar el almacen del vendedor");
            return true;
        }

        public class Map
        {
            public Map(EntityTypeBuilder<Seller> builder)
            {
                builder.HasKey(x => x.SellerId);
                builder.Property(x => x.SellerId).HasColumnName("SellerId");
                builder.Property(x => x.SellerName).HasColumnName("SellerName");
                builder.Property(x => x.WhsCode).HasColumnName("WhsCode");
                builder.Property(x => x.RegionId).HasColumnName("RegionId");
                builder.Property(x => x.CreateBy).HasColumnName("CreateBy");
                builder.Property(x => x.CreateDate).HasColumnName("CreateDate");
                builder.Property(x => x.UpdateBy).HasColumnName("UpdateBy");
                builder.Property(x => x.UpdateDate).HasColumnName("UpdateDate");
                builder.Property(x => x.Active).HasColumnName("Active");
                builder.ToTable("Seller");
            }
        }
    }
}
