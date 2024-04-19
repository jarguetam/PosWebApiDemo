using Pos.WebApi.Features.Suppliers.Entities;

namespace Pos.WebApi.Features.Suppliers.Dto
{
    public class SupplierDto: Supplier
    {
        public string CategoryName { get; set; }
        public string PayConditionName { get; set; }
        public int PayConditionDays { get; set; }
        public string CreateByName { get; set; }
    }
}
