using Pos.WebApi.Features.PurchasePayment.Dto;
using Pos.WebApi.Features.SalesPayment.Entities;
using System.Collections.Generic;

namespace Pos.WebApi.Features.SalesPayment.Dto
{
    public class PaymentSaleDto: PaymentSale
    {
        public string PayConditionName { get; set; }
        public string CreateByName { get; set; }
        public new List<PaymentSaleDetailDto> Detail { get; set; }
    }
}
