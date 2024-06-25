using Pos.WebApi.Features.Items.Entities;

namespace Pos.WebApi.Features.Items.Dto
{
    public class ItemWareHouseDto: ItemWareHouse
    {
        public string WhsName { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string UnitOfMeasureName { get; set; }
        public int UnitOfMeasureId { get; set; }
        public string ItemCategoryName { get; set; }
        public string BarCode { get; set; }
        public bool Tax { get; set; }
        public bool Active { get; set; }
        public decimal PriceSales { get; set; }
        public decimal Weight { get; set; }
        public int CustomerId { get; set; }
        public int ListPriceId { get; set; }
    }
}
