using System;

namespace Pos.WebApi.Features.Items.Dto
{
    public class ItemWareHouseViewModel
    {
        public int ItemId { get; set; }
        public string ItemCode { get; set; }
        public int WhsCode { get; set; }
        public string WhsName { get; set; }
        public DateTime DueDate { get; set; }
        public string ItemName { get; set; }
        public int UnitOfMeasureId { get; set; }
        public string UnitOfMeasureName { get; set; }
        public string ItemCategoryName { get; set; }
        public decimal Stock { get; set; }
        public decimal AvgPrice { get; set; }
        public decimal PriceSales { get; set; }
        public bool Tax { get; set; }
        public string BarCode { get; set; }
        public bool Active { get; set; }
        public int ListPriceId { get; set; }
    }
}
