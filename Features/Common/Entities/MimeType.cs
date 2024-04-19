using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pos.WebApi.Features.Common.Entities
{
    public class MimeType
    {
        public int MimeId { get; set; }
        public string Type { get; set; }
        public string Extension { get; set; }

        public class Map
        {
            public Map(EntityTypeBuilder<MimeType> builder)
            {
                builder.HasKey(x => x.MimeId);
                builder.Property(x => x.MimeId).HasColumnName("MimeId");
                builder.Property(x => x.Type).HasColumnName("Type");
                builder.Property(x => x.Extension).HasColumnName("Extension");
                builder.ToTable("MimeType");
            }
        }
    }
}
