﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pos.WebApi.Features.Users.Entities
{
    public class Role
    {
        public int RoleId { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }

        public class Map
        {
            public Map(EntityTypeBuilder<Role> builder)
            {
                builder.HasKey(x => x.RoleId);
                builder.Property(x => x.RoleId).HasColumnName("RoleId");
                builder.Property(x => x.Description).HasColumnName("Description");
                builder.Property(x => x.Active).HasColumnName("Active");
                builder.ToTable("Role");
            }
        }
    }
}
