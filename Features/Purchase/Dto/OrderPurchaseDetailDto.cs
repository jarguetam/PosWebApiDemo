using Pos.WebApi.Features.Purchase.Entities;
using System;

namespace Pos.WebApi.Features.Purchase.Dto
{
    public class OrderPurchaseDetailDto: OrderPurchaseDetail
    {
       public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string WhsName { get; set; }
        public string UnitOfMeasureName {get; set; }
    }
}
