using System.Collections.Generic;

namespace Pos.WebApi.Features.Reports.Dto
{
    public class ReportInventoryCategoryDto
    {
        public string ItemCategoryName { get; set; }
        public List<InventoryCategoryDetail> Detail { get ; set; }

    }

    public class InventoryCategoryDetail
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string ItemCategoryName { get; set; }
        public string ItemFamilyName { get; set; }
        public decimal Stock {  get; set; }
        public decimal AvgPrice { get; set; }
        public decimal Total { get; set; }
    }
}
