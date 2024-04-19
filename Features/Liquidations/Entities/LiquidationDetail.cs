using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.VisualBasic;
using Pos.WebApi.Features.Expenses.Entities;
using System;
using System.Text.Json.Serialization;

namespace Pos.WebApi.Features.Liquidations.Entities
{
    public class LiquidationDetail
    {
        public int LiquidationDetailId { get; set; }
        public int LiquidationId { get; set; }
        public int DocNum { get; set; }
        public string DocType { get; set; }
        public string Reference { get; set; }
        public DateTime DocDate { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public decimal DocTotal { get; set; }
        [JsonIgnore]
        public Liquidation Liquidation { get; set; }
        public class Map
        {
            public Map(EntityTypeBuilder<LiquidationDetail> builder)
            {
                builder.HasKey(x => x.LiquidationDetailId);
                builder.Property(x => x.LiquidationDetailId).HasColumnName("LiquidationDetailId");
                builder.Property(x => x.LiquidationId).HasColumnName("LiquidationId");
                builder.Property(x => x.DocNum).HasColumnName("DocNum");
                builder.Property(x => x.DocType).HasColumnName("DocType");
                builder.Property(x => x.Reference).HasColumnName("Reference");
                builder.Property(x => x.DocDate).HasColumnName("DocDate");
                builder.Property(x => x.CustomerCode).HasColumnName("CustomerCode");
                builder.Property(x => x.CustomerName).HasColumnName("CustomerName");
                builder.Property(x => x.DocTotal).HasColumnName("DocTotal");
                builder.HasOne(x => x.Liquidation).WithMany(x => x.Detail).HasForeignKey(x => x.LiquidationId);
                builder.ToTable("LiquidationDetail");
            }
        }
    }
}
