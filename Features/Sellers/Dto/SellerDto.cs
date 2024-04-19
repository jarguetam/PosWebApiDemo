using Pos.WebApi.Features.Sellers.Entities;

namespace Pos.WebApi.Features.Sellers.Dto
{
    public class SellerDto: Seller
    {
        public string RegionName { get; set; }
        public string WhsName { get; set; }
        public string CreateByName { get; set; }
    }
}
