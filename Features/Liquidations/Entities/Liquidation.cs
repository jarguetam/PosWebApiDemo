using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pos.WebApi.Features.Expenses.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pos.WebApi.Features.Liquidations.Entities
{
    public class Liquidation
    {
        public int IdLiquidation { get; set; }
        public int SellerId { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }    
        public decimal SaleCredit { get; set; }
        public decimal SaleCash { get; set; }
        public decimal SaleTotal { get; set; }
        public decimal PaidTotal { get; set; }
        public decimal ExpenseTotal { get; set; }
        public decimal Total { get; set; }
        public decimal Deposit { get; set; }
        public decimal TotalDifference { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public bool Active { get; set; }
        public List<LiquidationDetail> Detail { get; set; }
        public Liquidation()
        {
            Detail = new List<LiquidationDetail>();
        }

        public bool IsValid()
        {
            var existQtyWithZero = Detail.Count();
           if (existQtyWithZero == 0) throw new Exception("No existen documentos para aplicar esta liquidacion.");
            
            if (this.Deposit <= 0) throw new System.Exception("El deposito debe ser mayor a 0.");
            if (this.Total <= 0) throw new System.Exception("El Total Ingresos debe ser mayor a 0.");
            
            return true;
        }

        public class Map
        {
            public Map(EntityTypeBuilder<Liquidation> builder)
            {
                builder.HasKey(x => x.IdLiquidation);
                builder.Property(x => x.IdLiquidation).HasColumnName("IdLiquidation");
                builder.Property(x => x.SellerId).HasColumnName("SellerId");
                builder.Property(x => x.From).HasColumnName("From");
                builder.Property(x => x.To).HasColumnName("To");
                builder.Property(x => x.SaleCredit).HasColumnName("SaleCredit");
                builder.Property(x => x.SaleCash).HasColumnName("SaleCash");
                builder.Property(x => x.SaleTotal).HasColumnName("SaleTotal");
                builder.Property(x => x.PaidTotal).HasColumnName("PaidTotal");
                builder.Property(x => x.ExpenseTotal).HasColumnName("ExpenseTotal");
                builder.Property(x => x.Total).HasColumnName("Total");
                builder.Property(x => x.Deposit).HasColumnName("Deposit");
                builder.Property(x => x.TotalDifference).HasColumnName("TotalDifference");
                builder.Property(x => x.CreatedDate).HasColumnName("CreatedDate");
                builder.Property(x => x.CreatedBy).HasColumnName("CreatedBy");
                builder.Property(x => x.Active).HasColumnName("Active");
                builder.HasMany(x => x.Detail).WithOne(x => x.Liquidation).HasForeignKey(x => x.LiquidationId);
                builder.ToTable("Liquidation");
            }
        }
    }
}
