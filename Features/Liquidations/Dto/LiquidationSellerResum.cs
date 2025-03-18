using System.Collections.Generic;

namespace Pos.WebApi.Features.Liquidations.Dto
{

    public class LiquidationSellerDto
    {
        public List<LiquidationSellerResumDto> Resum { get; set; }
        public List<LiquidationSellerResumDetailDto> Detail { get; set; }
    }
    public class LiquidationSellerResumDto
    {
        public string DocType { get; set; }
        public decimal Total { get; set; }
    }

    public class LiquidationSellerResumDetailDto
    {
        public string DocTypeCode { get; set;}
        public string CustomerName { get; set;}
        public decimal Total { get; set;}
    }
}
