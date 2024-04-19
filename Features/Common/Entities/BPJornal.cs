using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pos.WebApi.Features.InventoryTransactions.Entities;
using System;

namespace Pos.WebApi.Features.Common.Entities
{
    public class BPJornal
    {
        public long BPJournalId { get; set; }
        public int BpId { get; set; }
        public string BpType { get; set; }
        public int DocId { get; set; }
        public decimal TransValue { get; set; }
        public string Documents { get; set; }
        public int DocumentReferent { get; set; }
        public int CreateBy { get; set; }
        public DateTime CreateDate { get; set; }

        public bool IsValid()
        {
            if ((this.BpId) == 0) throw new System.Exception("Debe venir el socio de negocio. Contactese con su administrador.");
            if ((this.DocId) == 0) throw new System.Exception("Debe venir el numero de documento. Contactese con su administrador.");
            if ((this.TransValue) == 0) throw new System.Exception("Debe venir el valor de la transaccion. Contactese con su administrador.");      
            return true;
        }

        public class Map
        {
            public Map(EntityTypeBuilder<BPJornal> builder)
            {
                builder.HasKey(x => x.BPJournalId);
                builder.Property(x => x.BPJournalId).HasColumnName("BPJournalId");
                builder.Property(x => x.BpId).HasColumnName("BpId");
                builder.Property(x => x.BpType).HasColumnName("BpType");
                builder.Property(x => x.DocId).HasColumnName("DocId");
                builder.Property(x => x.TransValue).HasColumnName("TransValue");
                builder.Property(x => x.Documents).HasColumnName("Documents");
                builder.Property(x => x.DocumentReferent).HasColumnName("DocumentReferent");
                builder.Property(x => x.CreateBy).HasColumnName("CreateBy");
                builder.Property(x => x.CreateDate).HasColumnName("CreateDate");
                builder.ToTable("BPJournal");
            }
        }
    }
}
