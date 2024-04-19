using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Pos.WebApi.Features.Sales.Entities
{
    public class InvoiceSale
    {
        public int DocId { get; set; }
        public int DocReference { get; set; }
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
        public string InvoiceFiscalNo { get; set; }
        public string Establishment { get; set; }
        public string Point { get; set; }
        public string Type { get; set; }
        public string Cai { get; set; }
        public DateTime LimitIssue { get; set; }
        public string AuthorizedRangeFrom { get; set; }
        public string AuthorizedRangeTo { get; set; }
        public int CorrelativeId { get; set; }
        public decimal PaidToDate { get; set; }
        public decimal Balance { get; set; }
        public List<InvoiceSaleDetail> Detail { get; set; }
        public InvoiceSale()
        {
            Detail = new List<InvoiceSaleDetail>();
        }
        public bool IsValid()
        {
            if ((this.WhsCode) == 0) throw new System.Exception("Error: Debe venir el almacen.");
            if ((this.DocTotal) == 0) throw new System.Exception("Error: El total del documento debe ser mayor a 0.");
            if (string.IsNullOrEmpty(this.Comment)) throw new System.Exception("Error: Debe agregar un comentario.");
            var existQtyWithZero = Detail.Where(x => x.Quantity == 0).Count();
            if (existQtyWithZero > 0) throw new Exception("Error: Existen articulos con cantidad 0");
            var existPriceWithZero = Detail.Where(x => x.Price == 0).Count();
            if (existPriceWithZero > 0) throw new Exception("Error: Existen articulos sin precio");
           // var duedate = Detail.Where(x => x.DueDate.Date <= DateTime.Now.Date).Count();
            //if (duedate > 0) throw new Exception("Fecha de vencimiento debe ser mayor a la fecha actual.");
            if(this.CorrelativeId ==0) throw new Exception("Error: Debe seleccionar la numeración.");
            return true;
        }

        public class Map
        {
            public Map(EntityTypeBuilder<InvoiceSale> builder)
            {
                builder.HasKey(x => x.DocId);
                builder.Property(x => x.DocId).HasColumnName("DocId");
                builder.Property(x => x.DocReference).HasColumnName("DocReference");
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
                builder.Property(x => x.InvoiceFiscalNo).HasColumnName("InvoiceFiscalNo");
                builder.Property(x => x.Establishment).HasColumnName("Establishment");
                builder.Property(x => x.Point).HasColumnName("Point");
                builder.Property(x => x.Type).HasColumnName("Type");
                builder.Property(x => x.Cai).HasColumnName("Cai");
                builder.Property(x => x.LimitIssue).HasColumnName("LimitIssue");
                builder.Property(x => x.AuthorizedRangeFrom).HasColumnName("AuthorizedRangeFrom");
                builder.Property(x => x.AuthorizedRangeTo).HasColumnName("AuthorizedRangeTo");
                builder.Property(x => x.CorrelativeId).HasColumnName("CorrelativeId");
                builder.Property(x => x.PaidToDate).HasColumnName("PaidToDate");
                builder.Property(x => x.Balance).HasColumnName("Balance");
                builder.HasMany(x => x.Detail).WithOne(x => x.InvoiceSale).HasForeignKey(x => x.DocId);
                builder.ToTable("InvoiceSale");
            }
        }
    }
}
