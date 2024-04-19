using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Pos.WebApi.Features.Items.Entities
{
    public class PriceListDetail
    {
        public int PriceListDetailId { get; set; }
        public int ListPriceId { get; set; }
        public int ItemId { get; set; } 
        public decimal Price { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public int UpdateBy { get; set; }
       
        public bool IsValid()
        {
            if ((this.ItemId ==0)) throw new System.Exception("No se pudo agregar a la lista de precio porque no viene el articulo.");
            return true;
        }

        public class Map
        {
            public Map(EntityTypeBuilder<PriceListDetail> builder)
            {
                builder.HasKey(x => x.PriceListDetailId);
                builder.Property(x => x.PriceListDetailId).HasColumnName("PriceListDetailId");
                builder.Property(x => x.ListPriceId).HasColumnName("ListPriceId");
                builder.Property(x => x.ItemId).HasColumnName("ItemId");
                builder.Property(x => x.CreateDate).HasColumnName("CreateDate");
                builder.Property(x => x.Price).HasColumnName("Price");
                builder.Property(x => x.UpdateBy).HasColumnName("UpdateBy");
                builder.Property(x => x.UpdateDate).HasColumnName("UpdateDate");
                builder.ToTable("PriceListDetail");
            }
        }
    }
}
