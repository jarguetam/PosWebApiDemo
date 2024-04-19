using Pos.WebApi.Features.InventoryTransactions.Entities;
using System.Runtime.CompilerServices;

namespace Pos.WebApi.Features.InventoryTransactions.Dto
{
    public class InventoryEntryDetailDto: InventoryEntryDetail
    {
        public string WhsName { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string UnitOfMeasureName { get; set; }
    }
}
