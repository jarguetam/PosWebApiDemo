using Pos.WebApi.Features.Expenses.Dto;
using Pos.WebApi.Features.Liquidations.Entities;
using System.Collections.Generic;

namespace Pos.WebApi.Features.Liquidations.Dto
{
    public class LiquidationDto:Liquidation
    {
        public string CreatedByName { get; set; }
        public string SellerName { get; set; }
       
    }
}
