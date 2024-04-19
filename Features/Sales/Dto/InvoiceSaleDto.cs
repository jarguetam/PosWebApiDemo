using Pos.WebApi.Features.Sales.Entities;
using Pos.WebApi.Features.SalesPayment.Dto;
using Pos.WebApi.Features.SalesPayment.Entities;
using System.Collections.Generic;

namespace Pos.WebApi.Features.Sales.Dto
{
    public class InvoiceSaleDto:InvoiceSale
    {
        public string PayConditionName { get; set; }
        public string CreateByName { get; set; }
        public string WhsName { get; set; }
        public bool TaxCustomer { get; set; }
        public string SellerName { get; set; }
        public List<InvoiceSaleDetailDto> DetailDto { get; set; }
        public string DeiNumber { get { return @$"{Establishment}-{Point}-{Type}-{InvoiceFiscalNo}"; } }
        public PaymentSale Payment { get; set; }
    }
}
