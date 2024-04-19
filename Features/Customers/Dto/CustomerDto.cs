using Pos.WebApi.Features.Customers.Entities;

namespace Pos.WebApi.Features.Customers.Dto
{
    public class CustomerDto: Customer
    {
        public string SellerName { get; set; }
        public string CategoryName { get; set; }
        public string PayConditionName { get; set; }
        public string PriceListName { get; set; }
        public int PayConditionDays { get; set; }
        public string CreateByName { get; set; }
        public string FrequencyName { get; set; }
        public string ZoneName { get; set; }
    }
}
