using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pos.WebApi.Features.Purchase.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pos.WebApi.Features.Expenses.Entities
{
    public class Expense
    {
        public int ExpenseId { get; set; }
        public DateTime ExpenseDate { get; set; }
        public string Comment { get; set; }
        public decimal Total { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int SellerId { get; set; }
        public bool Active { get; set; }
        public List<ExpenseDetail> Detail { get; set; }
        public Expense()
        {
            Detail = new List<ExpenseDetail>();
        }

        public bool IsValid()
        {
            var existQtyWithZero = Detail.Where(x => string.IsNullOrEmpty(x.Reference)).Count();
            if (existQtyWithZero > 0) throw new Exception("Existen lineas sin referencia.");
            if (string.IsNullOrEmpty(this.Comment)) throw new System.Exception("Debe ingresar un comentario.");
            return true;
        }

        public class Map
        {
            public Map(EntityTypeBuilder<Expense> builder)
            {
                builder.HasKey(x => x.ExpenseId);
                builder.Property(x => x.ExpenseId).HasColumnName("ExpenseId");
                builder.Property(x => x.ExpenseDate).HasColumnName("ExpenseDate");
                builder.Property(x => x.Comment).HasColumnName("Comment");
                builder.Property(x => x.Total).HasColumnName("Total");
                builder.Property(x => x.CreatedBy).HasColumnName("CreatedBy");
                builder.Property(x => x.SellerId).HasColumnName("SellerId");
                builder.Property(x => x.Active).HasColumnName("Active");
                builder.HasMany(x => x.Detail).WithOne(x => x.Expense).HasForeignKey(x => x.ExpenseId);
                builder.ToTable("Expense");
            }
        }
    }
}
