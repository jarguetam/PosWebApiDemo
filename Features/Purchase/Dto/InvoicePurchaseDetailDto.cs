using Pos.WebApi.Features.Purchase.Entities;

namespace Pos.WebApi.Features.Purchase.Dto
{
    public class InvoicePurchaseDetailDto: InvoicePurchaseDetail
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string WhsName { get; set; }
        public string UnitOfMeasureName { get; set; }
    }
}
