using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pos.WebApi.Features.Purchase.Entities;
using System.Collections.Generic;
using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Pos.WebApi.Features.Sales.Entities
{
    public class OrderSale
    {
        public int DocId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerRTN { get; set; }
        public string CustomerAddress { get; set; }
        public int PayConditionId { get; set; }
        public int SellerId { get; set; }
        public int PriceListId { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime DueDate { get; set; }
        public bool Canceled { get; set; }
        public string Comment { get; set; }
        public string Reference { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Disccounts { get; set; }
        public decimal DiscountsTotal { get; set; }
        public decimal DocTotal { get; set; }
        public decimal DocQty { get; set; }
        public int WhsCode { get; set; }
        public int CreateBy { get; set; }
        public bool Complete { get; set; }
        public new List<OrderSaleDetail> Detail { get; set; }
        public OrderSale()
        {
            Detail = new List<OrderSaleDetail>();
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
            var duedate = Detail.Where(x => x.DueDate.Date <= DateTime.Now.Date).Count();
            if (duedate > 0) throw new Exception("Fecha de vencimiento debe ser mayor a la fecha actual.");
            return true;
        }

        public class Map
        {
            public Map(EntityTypeBuilder<OrderSale> builder)
            {
                builder.HasKey(x => x.DocId);
                builder.Property(x => x.DocId).HasColumnName("DocId");
                builder.Property(x => x.CustomerId).HasColumnName("CustomerId");
                builder.Property(x => x.CustomerCode).HasColumnName("CustomerCode");
                builder.Property(x => x.CustomerName).HasColumnName("CustomerName");
                builder.Property(x => x.CustomerRTN).HasColumnName("CustomerRTN");
                builder.Property(x => x.CustomerAddress).HasColumnName("CustomerAddress");
                builder.Property(x => x.PayConditionId).HasColumnName("PayConditionId");
                builder.Property(x => x.SellerId).HasColumnName("SellerId");
                builder.Property(x => x.PriceListId).HasColumnName("PriceListId");
                builder.Property(x => x.DocDate).HasColumnName("DocDate");
                builder.Property(x => x.DueDate).HasColumnName("DueDate");
                builder.Property(x => x.Canceled).HasColumnName("Canceled");
                builder.Property(x => x.Comment).HasColumnName("Comment");
                builder.Property(x => x.Reference).HasColumnName("Reference");
                builder.Property(x => x.WhsCode).HasColumnName("WhsCode");
                builder.Property(x => x.SubTotal).HasColumnName("SubTotal");
                builder.Property(x => x.Tax).HasColumnName("Tax");
                builder.Property(x => x.Disccounts).HasColumnName("Disccounts");
                builder.Property(x => x.DiscountsTotal).HasColumnName("DiscountsTotal");
                builder.Property(x => x.DocTotal).HasColumnName("DocTotal");
                builder.Property(x => x.DocQty).HasColumnName("DocQty");
                builder.Property(x => x.Comment).HasColumnName("Comment");
                builder.Property(x => x.CreateBy).HasColumnName("CreateBy");
                builder.Property(x => x.Complete).HasColumnName("Complete");
                builder.HasMany(x => x.Detail).WithOne(x => x.OrderSale).HasForeignKey(x => x.DocId);
                builder.ToTable("OrderSale");
            }
        }
    }
}
