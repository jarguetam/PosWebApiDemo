using Pos.WebApi.Features.InventoryTransactions.Entities;

namespace Pos.WebApi.Features.InventoryTransactions.Dto
{
    public class InventoryReturnDto: InventoryReturn
    {
        public string SellerName { get; set; }
        public string RegionName { get; set; }
        public string WhsName { get; set; }
        public string CreatedByName { get; set; }
    }
}
