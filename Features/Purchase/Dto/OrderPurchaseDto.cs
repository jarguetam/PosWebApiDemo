using Pos.WebApi.Features.InventoryTransactions.Dto;
using Pos.WebApi.Features.Purchase.Entities;
using System.Collections.Generic;

namespace Pos.WebApi.Features.Purchase.Dto
{
    public class OrderPurchaseDto: OrderPurchase
    {
        public string PayConditionName { get; set; }
        public string CreateByName { get; set; }
        public string WhsName { get; set; }
        public bool taxSupplier { get; set; }
        public new List<OrderPurchaseDetailDto> Detail { get; set; }
    }
}
