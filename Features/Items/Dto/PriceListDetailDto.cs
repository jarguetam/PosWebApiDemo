using Org.BouncyCastle.Asn1.Mozilla;
using Pos.WebApi.Features.Items.Entities;
using System.Collections.Generic;

namespace Pos.WebApi.Features.Items.Dto
{
    public class PriceListDetailDto: PriceListDetail
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal PriceSpecial { get; set; }
        public bool IsModified { get; set; }
    }
}
