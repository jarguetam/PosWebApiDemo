using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Pos.WebApi.Features.Customers.Entities
{
    public class PriceList
    {
        public int ListPriceId { get; set; }
        public string ListPriceName { get; set; }
        public int PorcentGain { get; set; }
        public int CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public int UpdateBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool Active { get; set; }

        public bool IsValid()
        {
            if (string.IsNullOrEmpty(this.ListPriceName)) throw new System.Exception("Debe ingresar un nombre");
            if (this.PorcentGain<=0) throw new System.Exception("La ganancia deberia ser mayor a 0.");
            return true;
        }

        public class Map
        {
            public Map(EntityTypeBuilder<PriceList> builder)
            {
                builder.HasKey(x => x.ListPriceId);
                builder.Property(x => x.ListPriceId).HasColumnName("ListPriceId");
                builder.Property(x => x.ListPriceName).HasColumnName("ListPriceName");
                builder.Property(x => x.PorcentGain).HasColumnName("PorcentGain");
                builder.Property(x => x.CreateBy).HasColumnName("CreateBy");
                builder.Property(x => x.CreateDate).HasColumnName("CreateDate");
                builder.Property(x => x.UpdateBy).HasColumnName("UpdateBy");
                builder.Property(x => x.UpdateDate).HasColumnName("UpdateDate");
                builder.Property(x => x.Active).HasColumnName("Active");
                builder.ToTable("PriceList");
            }
        }
    }
}
