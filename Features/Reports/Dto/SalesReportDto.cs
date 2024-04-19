using System;

namespace Pos.WebApi.Features.Reports.Dto
{
    public class SalesReportDto
    {
        public string InvoiceFiscalNo { get; set; }
        public DateTime DocDate { get; set; }
        public string CustomerCode { get; set;}
        public string CustomerName { get; set;}
        public string PayConditionName { get; set;}
        public string ItemCode { get; set;}
        public string ItemName { get; set;}
        public decimal Quantity { get; set;}
        public decimal Price { get; set;}
        public decimal LineTotal { get; set;}
        public string WhsName { get; set;}
        public string SellerName { get; set;}
        public string SellerZone{ get;set;}
    }
}
