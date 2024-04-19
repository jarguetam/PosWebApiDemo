using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Pos.WebApi.Features.Customers.Entities
{
    public class CustomerZone
    {
        public int Id { get; set; }
        public string ZoneName { get; set; }
        public int CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
     

        public class Map
        {
            public Map(EntityTypeBuilder<CustomerZone> builder)
            {
                builder.HasKey(x => x.Id);        
                builder.Property(x => x.ZoneName).HasColumnName("ZoneName");
                builder.Property(x => x.CreateBy).HasColumnName("CreatedBy");
                builder.Property(x => x.CreateDate).HasColumnName("CreatedDate");              
                builder.ToTable("CustomerZone");
            }
        }
    }
}
