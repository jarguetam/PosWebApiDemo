using System;

namespace Pos.WebApi.Features.Liquidations.Entities
{
    public class LiquidationView
    {
        public long Id { get; set; }
        public int DocNum { get; set; }
        public string DocType { get; set; }
        public string Reference { get; set; }
        public DateTime DocDate { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public decimal DocTotal { get; set; }
        public int SellerId { get; set; }
    }
}
