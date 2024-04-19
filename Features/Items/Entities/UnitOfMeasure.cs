using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Pos.WebApi.Features.Items.Entities
{
    public class UnitOfMeasure
    {
        public int UnitOfMeasureId { get; set; }
        public string UnitOfMeasureName { get; set; }
        public int CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public int UpdateBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool Active { get; set; }

        public bool IsValid()
        {
            if (string.IsNullOrEmpty(this.UnitOfMeasureName)) throw new System.Exception("Debe ingresar un nombre");
            return true;
        }

        public class Map
        {
            public Map(EntityTypeBuilder<UnitOfMeasure> builder)
            {
                builder.HasKey(x => x.UnitOfMeasureId);
                builder.Property(x => x.UnitOfMeasureId).HasColumnName("UnitOfMeasureId");
                builder.Property(x => x.UnitOfMeasureName).HasColumnName("UnitOfMeasureName");
                builder.Property(x => x.CreateBy).HasColumnName("CreateBy");
                builder.Property(x => x.CreateDate).HasColumnName("CreateDate");
                builder.Property(x => x.UpdateBy).HasColumnName("UpdateBy");
                builder.Property(x => x.UpdateDate).HasColumnName("UpdateDate");
                builder.Property(x => x.Active).HasColumnName("Active");
                builder.ToTable("UnitOfMeasure");
            }
        }
    }
}
