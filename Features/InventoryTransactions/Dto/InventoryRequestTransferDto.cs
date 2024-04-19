using Pos.WebApi.Features.InventoryTransactions.Entities;
using System.Collections.Generic;

namespace Pos.WebApi.Features.InventoryTransactions.Dto
{
    public class InventoryRequestTransferDto : InventoryRequestTransfer
    {
        public string FromWhsName { get; set; }
        public string ToWhsName { get; set; }
        public string CreateByName { get; set; }
        public decimal DocQuantity { get; set; }
        public new List<InventoryRequestTransferDetailDto> Detail { get; set; }
    }
}
