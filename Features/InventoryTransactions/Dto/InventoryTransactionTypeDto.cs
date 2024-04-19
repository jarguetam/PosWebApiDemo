using Pos.WebApi.Features.InventoryTransactions.Entities;

namespace Pos.WebApi.Features.InventoryTransactions.Dto
{
    public class InventoryTransactionTypeDto: InventoryTransactionType
    {
        public string CreateByName { get; set; }
    }
}
