using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pos.WebApi.Features.Purchase.Entities;
using System.Text.Json.Serialization;

namespace Pos.WebApi.Features.Expenses.Entities
{
    public class ExpenseDetail
    {
        public int ExpenseDetailId { get; set; }
        public int ExpenseId { get; set; }
        public int ExpenseTypeId { get; set; }
        public string Reference { get; set; }
        public decimal LineTotal { get; set; }
        public bool IsDeleted { get; set; }
        [JsonIgnore]
        public Expense Expense { get; set; }

        public bool IsValid()
        {
            if (string.IsNullOrEmpty(this.Reference)) throw new System.Exception("Debe ingresar la referencia.");
            
            return true;
        }

        public class Map
        {
            public Map(EntityTypeBuilder<ExpenseDetail> builder)
            {
                builder.HasKey(x => x.ExpenseDetailId);
                builder.Property(x => x.ExpenseDetailId).HasColumnName("ExpenseDetailId");
                builder.Property(x => x.ExpenseId).HasColumnName("ExpenseId");
                builder.Property(x => x.ExpenseTypeId).HasColumnName("ExpenseTypeId");
                builder.Property(x => x.Reference).HasColumnName("Reference");
                builder.Property(x => x.LineTotal).HasColumnName("LineTotal");
                builder.Property(x => x.IsDeleted).HasColumnName("IsDeleted");
                builder.HasOne(x => x.Expense).WithMany(x => x.Detail).HasForeignKey(x => x.ExpenseId);
                builder.ToTable("ExpenseDetail");
            }
        }
    }
}
