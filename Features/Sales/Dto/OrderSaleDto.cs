using Pos.WebApi.Features.Sales.Entities;
using System.Collections.Generic;

namespace Pos.WebApi.Features.Sales.Dto
{
    public class OrderSaleDto: OrderSale
    {
        public string PayConditionName { get; set; }
        public string CreateByName { get; set; }
        public string WhsName { get; set; }
        public bool TaxCustomer { get; set; }
        public string SellerName { get; set; }
        public new List<OrderSaleDetailDto> Detail { get; set; }
    }
}
