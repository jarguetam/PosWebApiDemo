using Pos.WebApi.Features.InventoryTransactions.Entities;

namespace Pos.WebApi.Features.InventoryTransactions.Dto
{
    public class ItemJournalDto: ItemJournal
    {
        public string WhsName { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string CreateByName { get; set; }
    }
}
