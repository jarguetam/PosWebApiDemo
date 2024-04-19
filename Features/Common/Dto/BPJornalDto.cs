using Pos.WebApi.Features.Common.Entities;

namespace Pos.WebApi.Features.Common.Dto
{
    public class BPJornalDto: BPJornal
    {
        public string BusinnesPartnersCode { get; set; }
        public string BusinnesName { get; set; }
        public string CreateByName { get; set; } 
        public decimal Balance { get; set; }
    }
}
