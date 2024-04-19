using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pos.WebApi.Features.Common.Entities
{
    public class SarBranch
    {
        public string BranchId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public bool Active { get; set; }
        public int UserId { get; set; }
        public DateTime CreateDate { get; set; }
        public class Map
        {
            public Map(EntityTypeBuilder<SarBranch> builder)
            {
                builder.HasKey(x => x.BranchId);
                builder.Property(x => x.BranchId).HasColumnName("BranchId");
                builder.Property(x => x.Name).HasColumnName("Name");
                builder.Property(x => x.Address).HasColumnName("Address");
                builder.Property(x => x.Active).HasColumnName("Active");
                builder.Property(x => x.UserId).HasColumnName("UserId");
                builder.Property(x => x.CreateDate).HasColumnName("CreateDate");
                builder.ToTable("SarBranch");
            }
        }
    }
}
