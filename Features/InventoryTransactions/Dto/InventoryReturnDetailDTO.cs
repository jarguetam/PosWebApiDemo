using Pos.WebApi.Features.InventoryTransactions.Entities;

namespace Pos.WebApi.Features.InventoryTransactions.Dto
{
    public class InventoryReturnDetailDTO
    {
        public int IdDetail { get; set; }
        public int IdReturn { get; set; }
        public int ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal QuantityInitial { get; set; }
        public decimal QuantityWareHouse { get; set; }
        public decimal QuantitySeller { get; set; }
        public decimal QuantityOutPut { get; set; }
        public decimal QuantityReturn { get; set; }
        public decimal QuantityDiference { get; set; }
        public string Comment { get; set; }
        public string ItemCategoryName { get; set; }
    }
}
