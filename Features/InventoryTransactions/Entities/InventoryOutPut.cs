using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Pos.WebApi.Features.InventoryTransactions.Entities
{
    public class InventoryOutPut
    {
        public int OutputId { get; set; }
        public DateTime OutputDate { get; set; }
        public string Comment { get; set; }
        public decimal DocTotal { get; set; }
        public int Type { get; set; }
        public int WhsCode { get; set; }
        public int CreateBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<InventoryOutPutDetail> Detail { get; set; }
        public InventoryOutPut()
        {
            Detail = new List<InventoryOutPutDetail>();
        }
        public bool IsValid()
        {
            if ((this.WhsCode) == 0) throw new System.Exception("Debe venir el almacen.");
            if ((this.DocTotal) == 0) throw new System.Exception("El total del documento debe ser mayor a 0.");
            if (string.IsNullOrEmpty(this.Comment)) throw new System.Exception("Debe agregar un comentario.");
            var existQtyWithZero = Detail.Where(x => x.Quantity == 0).Count();
            if (existQtyWithZero > 0) throw new Exception("Existen articulos con cantidad 0");
            var existPriceWithZero = Detail.Where(x => x.Price == 0).Count();
            if (existPriceWithZero > 0) throw new Exception("Existen articulos sin precio");
            if(this.Detail.Count==0) throw new System.Exception("Debe agregar un producto.");
            //var duedate = Detail.Where(x => x.DueDate.Date <= DateTime.Now.Date).Count();
            //if (duedate > 0) throw new Exception("Fecha de vencimiento debe ser mayor a la fecha actual.");
            return true;
        }

        public class Map
        {
            public Map(EntityTypeBuilder<InventoryOutPut> builder)
            {
                builder.HasKey(x => x.OutputId);
                builder.Property(x => x.OutputId).HasColumnName("OutputId");
                builder.Property(x => x.OutputDate).HasColumnName("OutputDate");
                builder.Property(x => x.WhsCode).HasColumnName("WhsCode");
                builder.Property(x => x.Type).HasColumnName("Type");
                builder.Property(x => x.Comment).HasColumnName("Comment");
                builder.Property(x => x.CreateBy).HasColumnName("CreateBy");
                builder.Property(x => x.CreatedDate).HasColumnName("CreatedDate");
                builder.HasMany(x => x.Detail).WithOne(x => x.InventoryOutPut).HasForeignKey(x => x.OutputId);
                builder.ToTable("InventoryOutPut");
            }
        }
    }
}
