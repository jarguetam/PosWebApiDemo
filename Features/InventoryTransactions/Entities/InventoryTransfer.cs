using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Pos.WebApi.Features.InventoryTransactions.Entities
{
    public class InventoryTransfer
    {
        public int TransferId { get; set; }
        public DateTime TransferDate { get; set; }
        public string Comment { get; set; }
        public decimal DocTotal { get; set; }
        public decimal QtyTotal { get; set; }
        public int FromWhsCode { get; set; }
        public int ToWhsCode { get; set; }
        public int CreateBy { get; set; }
        public int TransferRequestId { get; set; }
        public List<InventoryTransferDetail> Detail { get; set; }
        public InventoryTransfer()
        {
            Detail = new List<InventoryTransferDetail>();
        }
        public bool IsValid()
        {
            if ((this.FromWhsCode) == (this.ToWhsCode)) throw new System.Exception("El almacen origen no puede ser el mismo almacen destino.");
            if ((this.FromWhsCode) == 0) throw new System.Exception("Debe seleccionar el almacen de origen.");
            if ((this.ToWhsCode) == 0) throw new System.Exception("Debe seleccionar el almacen de destino.");
            if ((this.DocTotal) == 0) throw new System.Exception("El total del documento debe ser mayor a 0.");
            if ((this.QtyTotal) == 0) throw new System.Exception("La cantidad debe ser mayor a 0.");
            if (string.IsNullOrEmpty(this.Comment)) throw new System.Exception("Debe agregar un comentario.");
            var existQtyWithZero = Detail.Where(x => x.Quantity == 0).Count();
            if (existQtyWithZero > 0) throw new Exception("Existen articulos con cantidad 0");
            var existPriceWithZero = Detail.Where(x => x.Price == 0).Count();
            if (existPriceWithZero > 0) throw new Exception("Existen articulos sin precio");
            var duedate = Detail.Where(x => x.DueDate.Date <= DateTime.Now.Date).Count();
            if (duedate > 0) throw new Exception("No puede transferir este producto. La Fecha de vencimiento debe ser mayor a la fecha actual.");
            return true;
        }

        public class Map
        {
            public Map(EntityTypeBuilder<InventoryTransfer> builder)
            {
                builder.HasKey(x => x.TransferId);
                builder.Property(x => x.TransferId).HasColumnName("TransferId");
                builder.Property(x => x.TransferRequestId).HasColumnName("TransferRequestId");
                builder.Property(x => x.TransferDate).HasColumnName("TransferDate");
                builder.Property(x => x.FromWhsCode).HasColumnName("FromWhsCode");
                builder.Property(x => x.ToWhsCode).HasColumnName("ToWhsCode");
                builder.Property(x => x.Comment).HasColumnName("Comment");
                builder.Property(x => x.CreateBy).HasColumnName("CreateBy");
                builder.HasMany(x => x.Detail).WithOne(x => x.InventoryTransfer).HasForeignKey(x => x.TransferId);
                builder.ToTable("InventoryTransfer");
            }
        }
    }
}
