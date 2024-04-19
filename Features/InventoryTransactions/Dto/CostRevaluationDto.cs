using Pos.WebApi.Features.InventoryTransactions.Entities;

namespace Pos.WebApi.Features.InventoryTransactions.Dto
{
    public class CostRevaluationDto:CostRevaluation
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }    
        public string WhsName { get; set; }
        public string CreateByName { get; set; }

    }
}
