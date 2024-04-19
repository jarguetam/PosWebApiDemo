using Pos.WebApi.Features.InventoryTransactions.Entities;
using System.Collections.Generic;

namespace Pos.WebApi.Features.InventoryTransactions.Dto
{
    public class InventoryEntryDto: InventoryEntry
    {
        public string WhsName { get; set; }
        public string CreateByName { get; set; }
        public decimal DocQuantity { get; set; }
        public string TypeName { get; set; }
        public new List<InventoryEntryDetailDto> Detail { get; set; }
    }
}
