using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Pos.WebApi.Features.Expenses.Entities
{
    public class ExpenseType
    {
        public int ExpenseTypeId { get; set; }
        public string ExpenseName { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UpdateBy { get; set; }
        public DateTime UpdateDate { get; set; }

        public bool IsValid()
        {
            if (string.IsNullOrEmpty(this.ExpenseName)) throw new System.Exception("Debe ingresar un nombre");
            return true;
        }

        public class Map
        {
            public Map(EntityTypeBuilder<ExpenseType> builder)
            {
                builder.HasKey(x => x.ExpenseTypeId);
                builder.Property(x => x.ExpenseTypeId).HasColumnName("ExpenseTypeId");
                builder.Property(x => x.ExpenseName).HasColumnName("ExpenseName");
                builder.Property(x => x.CreatedDate).HasColumnName("CreatedDate");
                builder.Property(x => x.CreatedBy).HasColumnName("CreatedBy");
                builder.Property(x => x.UpdateDate).HasColumnName("UpdateDate");
                builder.Property(x => x.UpdateBy).HasColumnName("UpdateBy");
                builder.ToTable("ExpenseType");
            }
        }
    }
}
