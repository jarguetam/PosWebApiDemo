using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pos.WebApi.Features.Common.Entities
{
    public class TypeEntry
    {
        public int TypeEntryId { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public class Map
        {
            public Map(EntityTypeBuilder<TypeEntry> builder)
            {
                builder.HasKey(x => x.TypeEntryId);
                builder.Property(x => x.TypeEntryId).HasColumnName("Id_Type_Entry");
                builder.Property(x => x.Name).HasColumnName("Name");
                builder.Property(x => x.Active).HasColumnName("Active");
                builder.ToTable("TypeEntry");
            }
        }
    }
}
