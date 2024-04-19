using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Pos.WebApi.Features.Common.Entities
{
    public class PayCondition
    {
        public int PayConditionId { get; set; }
        public string PayConditionName { get; set; }
        public int PayConditionDays { get; set; }
        public int CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public int UpdateBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool Active { get; set; }

        public bool IsValid()
        {
            if (string.IsNullOrEmpty(this.PayConditionName)) throw new System.Exception("Debe ingresar un nombre");         
            return true;
        }

        public class Map
        {
            public Map(EntityTypeBuilder<PayCondition> builder)
            {
                builder.HasKey(x => x.PayConditionId);
                builder.Property(x => x.PayConditionId).HasColumnName("PayConditionId");
                builder.Property(x => x.PayConditionName).HasColumnName("PayConditionName");
                builder.Property(x => x.PayConditionDays).HasColumnName("PayConditionDays");
                builder.Property(x => x.CreateBy).HasColumnName("CreateBy");
                builder.Property(x => x.CreateDate).HasColumnName("CreateDate");
                builder.Property(x => x.UpdateBy).HasColumnName("UpdateBy");
                builder.Property(x => x.UpdateDate).HasColumnName("UpdateDate");
                builder.Property(x => x.Active).HasColumnName("Active");
                builder.ToTable("PayCondition");
            }
        }
    }
}
