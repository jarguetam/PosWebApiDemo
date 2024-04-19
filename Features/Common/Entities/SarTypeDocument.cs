using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pos.WebApi.Features.Common.Entities
{
    public class SarTypeDocument
    {
        public string TypeDocument { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
        public DateTime CreateDate { get; set; }
        public class Map
        {
            public Map(EntityTypeBuilder<SarTypeDocument> builder)
            {
                builder.HasKey(x => x.TypeDocument);
                builder.Property(x => x.TypeDocument).HasColumnName("TypeDocument");
                builder.Property(x => x.Name).HasColumnName("Name");
                builder.Property(x => x.UserId).HasColumnName("UserId");
                builder.Property(x => x.CreateDate).HasColumnName("CreateDate");
                builder.ToTable("SarTypeDocument");
            }
        }
    }
}
