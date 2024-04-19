using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Pos.WebApi.Features.Customers.Entities
{
    public class PriceSpecialCustomerDetail
    {
        public int PriceSpecialId { get; set; }
        public int CustomerId { get; set; }
        public int ItemId { get; set; }
        public decimal PriceSpecial { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public int PriceListId { get; set; }
        public int CreateBy { get; set; }
        public int UpdateBy { get; set; }

        public bool IsValid()
        {
            if ((this.ItemId == 0)) throw new System.Exception("No se pudo agregar a la lista de precio porque no viene el articulo.");
            return true;
        }

        public class Map
        {
            public Map(EntityTypeBuilder<PriceSpecialCustomerDetail> builder)
            {
                builder.HasKey(x => x.PriceSpecialId);
                builder.Property(x => x.PriceSpecialId).HasColumnName("PriceSpecialId");
                builder.Property(x => x.CustomerId).HasColumnName("CustomerId");
                builder.Property(x => x.ItemId).HasColumnName("ItemId");
                builder.Property(x => x.CreateDate).HasColumnName("CreateDate");
                builder.Property(x => x.PriceSpecial).HasColumnName("PriceSpecial");
                builder.Property(x => x.PriceListId).HasColumnName("PriceListId");
                builder.Property(x => x.UpdateBy).HasColumnName("UpdateBy");
                builder.Property(x => x.UpdateDate).HasColumnName("UpdateDate");
                builder.ToTable("PriceSpecialCustomerDetail");
            }
        }
    }
}
