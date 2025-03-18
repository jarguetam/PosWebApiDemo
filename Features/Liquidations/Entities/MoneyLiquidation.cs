using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pos.WebApi.Features.Users.Entities;
using System.Collections.Generic;
using System;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq;
using Pos.WebApi.Features.Sellers.Entities;

namespace Pos.WebApi.Features.Liquidations.Entities
{
    public class MoneyLiquidation
    {
        public int LiquidationId { get; set; }
        public DateTime ExpenseDate { get; set; }
        public string Comment { get; set; }
        public decimal Total { get; set; }
        public decimal Deposit { get; set; }
        public int SellerId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public Seller Seller {  get; set; }
        public List<MoneyLiquidationDetail> Details { get; set; }

        public MoneyLiquidation()
        {
            Details = new List<MoneyLiquidationDetail>();
        }

        public class Map
        {
            public Map(EntityTypeBuilder<MoneyLiquidation> builder)
            {
                builder.HasKey(x => x.LiquidationId);
                builder.Property(x => x.LiquidationId).HasColumnName("LiquidationId");
                builder.Property(x => x.ExpenseDate).HasColumnName("ExpenseDate").IsRequired();
                builder.Property(x => x.Comment).HasColumnName("Comment");
                builder.Property(x => x.Total).HasColumnName("Total").HasColumnType("DECIMAL(18, 2)");
                builder.Property(x => x.Deposit).HasColumnName("TotalDeposit").HasColumnType("DECIMAL(18, 2)");
                builder.Property(x => x.SellerId).HasColumnName("SellerId").IsRequired();
                builder.Property(x => x.CreatedBy).HasColumnName("CreatedBy").IsRequired();
                builder.Property(x => x.CreatedAt).HasColumnName("CreatedAt").HasDefaultValueSql("GETDATE()");
                builder.Property(x => x.UpdatedAt).HasColumnName("UpdatedAt");
                builder.HasMany(x => x.Details).WithOne(x => x.MoneyLiquidation).HasForeignKey(x => x.LiquidationId);
                builder.HasOne(x => x.Seller)
               .WithMany()
               .HasForeignKey(x => x.SellerId);

                builder.ToTable("MoneyLiquidation");
            }
        }

        public bool IsValid()
        {
            var existQtyWithZero = Details.Count();
            if (existQtyWithZero == 0) throw new Exception("No existen billetes para aplicar esta liquidacion.");
            if (this.Total <= 0) throw new System.Exception("El Total Ingresos debe ser mayor a 0.");

            return true;
        }
    }
}
