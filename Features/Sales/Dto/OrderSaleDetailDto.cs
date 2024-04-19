using Pos.WebApi.Features.Sales.Entities;

namespace Pos.WebApi.Features.Sales.Dto
{
    public class OrderSaleDetailDto: OrderSaleDetail
    {
       public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string WhsName { get; set; }
        public string UnitOfMeasureName { get; set; }
    }
}
