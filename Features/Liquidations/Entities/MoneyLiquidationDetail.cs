using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;


namespace Pos.WebApi.Features.Liquidations.Entities
{
    public class MoneyLiquidationDetail
    {
        public int DetailId { get; set; }
        public int LiquidationId { get; set; }
        public int MoneyId { get; set; }
        public string Denominacion { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }

        // Navegación
        [JsonIgnore]
        public MoneyLiquidation MoneyLiquidation { get; set; }

        public class Map
        {
            public Map(EntityTypeBuilder<MoneyLiquidationDetail> builder)
            {
                builder.HasKey(x => x.DetailId);
                builder.Property(x => x.DetailId).HasColumnName("DetailId");
                builder.Property(x => x.LiquidationId).HasColumnName("LiquidationId").IsRequired();
                builder.Property(x => x.MoneyId).HasColumnName("MoneyId").IsRequired();
                builder.Property(x => x.Denominacion).HasColumnName("Denominacion").IsRequired();
                builder.Property(x => x.Quantity).HasColumnName("Quantity").IsRequired();
                builder.Property(x => x.Total).HasColumnName("Total").HasColumnType("DECIMAL(18, 2)").IsRequired();
                builder.HasOne(x => x.MoneyLiquidation).WithMany(x => x.Details).HasForeignKey(x => x.LiquidationId);
                builder.ToTable("MoneyLiquidationDetail");
            }
        }
    }
}
