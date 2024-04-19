using Pos.WebApi.Features.InventoryTransactions.Entities;
using System.Runtime.CompilerServices;

namespace Pos.WebApi.Features.InventoryTransactions.Dto
{
    public class InventoryRequestTransferDetailDto : InventoryRequestTransferDetail
    {
        public string FromWhsName { get; set; }
        public string ToWhsName { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }   
        public string UnitOfMeasureName { get; set; }
    }
}
