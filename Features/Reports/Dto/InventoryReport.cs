namespace Pos.WebApi.Features.Reports.Dto
{
    public class InventoryReportDto
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string WhsName { get; set; }
        public decimal Quantity { get; set; }
        public decimal AvgPrice { get; set; }
        public decimal CostTotal { get; set; }
    }
}
