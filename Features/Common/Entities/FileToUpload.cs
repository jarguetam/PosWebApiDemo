using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pos.WebApi.Features.Common.Dto
{
    public class FileUpload
    {
        public long FileId { get; set; }
        public string Description { get; set; }
        public string Extension { get; set; }
        public string Name { get; set; }

        public class Map
        {
            public Map(EntityTypeBuilder<FileUpload> builder)
            {
                builder.HasKey(x => x.FileId);
                builder.Property(x => x.FileId).HasColumnName("FileId");
                builder.Property(x => x.Description).HasColumnName("Description");
                builder.Property(x => x.Extension).HasColumnName("Extension");
                builder.Property(x => x.Name).HasColumnName("Name");
                builder.ToTable("FileUpload");
            }
        }
    }
}
