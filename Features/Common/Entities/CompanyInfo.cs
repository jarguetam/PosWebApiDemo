using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pos.WebApi.Features.Common.Entities
{
    public class CompanyInfo
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string Rtn { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Email2 { get; set; }
        public string Email1 { get; set; }
        public int UserId { get; set; }
        public long FileId { get; set; }
        public DateTime CreateDate { get; set; }
        public decimal TaxValue { get; set; } 
        public bool NegativeInventory { get; set; }
        public bool PrintLetter { get; set; }
        public class Map
        {
            public Map(EntityTypeBuilder<CompanyInfo> builder)
            {
                builder.HasKey(x => x.Id);
                builder.Property(x => x.Id).HasColumnName("Id");
                builder.Property(x => x.CompanyName).HasColumnName("CompanyName");
                builder.Property(x => x.Rtn).HasColumnName("Rtn");
                builder.Property(x => x.AddressLine1).HasColumnName("AddressLine1");
                builder.Property(x => x.AddressLine2).HasColumnName("AddressLine2");
                builder.Property(x => x.Phone1).HasColumnName("Phone1");
                builder.Property(x => x.Phone2).HasColumnName("Phone2");
                builder.Property(x => x.Email2).HasColumnName("Email2");
                builder.Property(x => x.Email1).HasColumnName("Email1");
                builder.Property(x => x.UserId).HasColumnName("UserId");
                builder.Property(x => x.CreateDate).HasColumnName("CreateDate");
                builder.Property(x => x.FileId).HasColumnName("FileId");
                builder.Property(x => x.TaxValue).HasColumnName("TaxValue");
                builder.Property(x => x.NegativeInventory).HasColumnName("NegativeInventory");
                builder.Property(x => x.PrintLetter).HasColumnName("PrintLetter");
                builder.ToTable("CompanyInfo");
            }
        }
    }
}
