using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Security.Claims;

namespace Pos.WebApi.Features.Liquidations.Entities
{
    public class MoneyBill
    {
        public int Id { get; set; }
        public decimal Denominacion { get; set; }
        public bool Activo { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdateDate { get; set; }

        public class Map
        {
            public Map(EntityTypeBuilder<MoneyBill> builder)
            {
                builder.HasKey(x => x.Id);
                builder.Property(x => x.Denominacion).HasColumnName("Denominacion");
                builder.Property(x => x.Activo).HasColumnName("Activo");
                builder.Property(x => x.CreatedDate).HasColumnName("CreatedDate");
                builder.Property(x => x.UpdateDate).HasColumnName("UpdateDate");
                builder.ToTable("Moneybills");
            }
        }
    }

    

}
