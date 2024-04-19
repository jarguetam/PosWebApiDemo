using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pos.WebApi.Features.Common.Entities
{
    public class SarCorrelative
    {
        public int CorrelativeId { get; set; }
        public int AuthorizeRangeFrom { get; set; }
        public int AuthorizeRangeTo { get; set; }
        public int CurrentCorrelative { get; set; }
        public string Cai { get; set; }
        public string BranchId { get; set; }
        public string PointSaleId { get; set; }
        public string TypeDocument { get; set; }
        public DateTime DateLimit { get; set; }
        public int UserId { get; set; }
        public DateTime CreateDate { get; set; }
        public string Description { get; set; }

        public bool IsValid()
        {
            if (string.IsNullOrEmpty(this.TypeDocument) || this.TypeDocument.Equals("00") ) throw new System.Exception("Debe seleccionar el tipo de documento.");
            if (string.IsNullOrEmpty(this.PointSaleId)) throw new System.Exception("Debe seleccionar el punto de venta.");
            if (string.IsNullOrEmpty(this.BranchId)) throw new System.Exception("Debe selecionar la sucursal.");

            return true;
        }
        public class Map
        {
            public Map(EntityTypeBuilder<SarCorrelative> builder)
            {
                builder.HasKey(x => x.CorrelativeId);
                builder.Property(x => x.CorrelativeId).HasColumnName("CorrelativeId");
                builder.Property(x => x.AuthorizeRangeFrom).HasColumnName("AuthorizeRangeFrom");
                builder.Property(x => x.AuthorizeRangeTo).HasColumnName("AuthorizeRangeTo");
                builder.Property(x => x.CurrentCorrelative).HasColumnName("CurrentCorrelative");
                builder.Property(x => x.Cai).HasColumnName("Cai");
                builder.Property(x => x.BranchId).HasColumnName("BranchId");
                builder.Property(x => x.PointSaleId).HasColumnName("PointSaleId");
                builder.Property(x => x.TypeDocument).HasColumnName("TypeDocument");
                builder.Property(x => x.DateLimit).HasColumnName("DateLimit");
                builder.Property(x => x.UserId).HasColumnName("UserId");
                builder.Property(x => x.CreateDate).HasColumnName("CreateDate");
                builder.Property(x => x.Description).HasColumnName("Description");
                builder.ToTable("SarCorrelative");
            }
        }
    }
}
