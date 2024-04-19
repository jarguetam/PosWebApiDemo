using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Pos.WebApi.Features.Customers.Entities
{
    public class CustomerFrequency
    {
        public int Id { get; set; }
        public string FrequencyName { get; set; }
        public int CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
 

        public class Map
        {
            public Map(EntityTypeBuilder<CustomerFrequency> builder)
            {
                builder.HasKey(x => x.Id);
                builder.Property(x => x.FrequencyName).HasColumnName("FrequencyName");
                builder.Property(x => x.CreateBy).HasColumnName("CreatedBy");
                builder.Property(x => x.CreateDate).HasColumnName("CreatedDate");       
                builder.ToTable("CustomerFrequency");
            }
        }
    }
}
