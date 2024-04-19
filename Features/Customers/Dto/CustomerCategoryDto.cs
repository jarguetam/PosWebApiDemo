using Pos.WebApi.Features.Customers.Entities;

namespace Pos.WebApi.Features.Customers.Dto
{
    public class CustomerCategoryDto: CustomerCategory
    {
        public string CreateByName { get; set; }
    }
}
