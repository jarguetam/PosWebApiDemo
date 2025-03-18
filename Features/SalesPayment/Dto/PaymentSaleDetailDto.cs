using Pos.WebApi.Features.SalesPayment.Entities;

namespace Pos.WebApi.Features.SalesPayment.Dto
{
    public class PaymentSaleDetailDto: PaymentSaleDetail
    {
        public decimal Balance { get; set; }
    }
}
