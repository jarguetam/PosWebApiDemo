using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pos.WebApi.Features.Reports.Dto
{
    public class ReportView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Params { get; set; }
        public string ViewName { get; set; }


        public class Map
        {
            public Map(EntityTypeBuilder<ReportView> builder)
            {
                builder.HasKey(x => x.Id);
                // Add your property mappings here
                 builder.Property(x => x.Name).HasColumnName("Name");
                 builder.Property(x => x.Params).HasColumnName("Params");
                 builder.Property(x => x.ViewName).HasColumnName("ViewName");
                builder.ToTable("ReportView");
            }
        }

    }
}
