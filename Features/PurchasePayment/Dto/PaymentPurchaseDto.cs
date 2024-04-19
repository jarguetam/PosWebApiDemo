using Pos.WebApi.Features.Purchase.Dto;
using Pos.WebApi.Features.PurchasePayment.Entitie;
using System.Collections.Generic;

namespace Pos.WebApi.Features.PurchasePayment.Dto
{
    public class PaymentPurchaseDto: PaymentPurchase
    {
        public string PayConditionName { get; set; }
        public string CreateByName { get; set; }
        public new List<PaymentPurchaseDetailDto> Detail { get; set; }
    }
}
