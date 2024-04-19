using Pos.WebApi.Features.Customers.Entities;

namespace Pos.WebApi.Features.Customers.Dto
{
    public class PriceListDto: PriceList
    {
        public string CreateByName { get; set; }
    }
}
