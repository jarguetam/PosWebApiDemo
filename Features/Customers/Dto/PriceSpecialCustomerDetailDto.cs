using Pos.WebApi.Features.Customers.Entities;

namespace Pos.WebApi.Features.Customers.Dto
{
    public class PriceSpecialCustomerDetailDto: PriceSpecialCustomerDetail
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
    }
}
