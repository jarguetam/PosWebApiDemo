using System;

namespace Pos.WebApi.Features.Sales.Dto
{
    public class CustomerAccountDto
    {
        public int      CustomerId { get; set; }
        public string   CustomerCode { get; set; }
        public string   CustomerName { get; set; }
        public string PayConditionName { get; set; }
        public string Uuid { get; set; }
        public int PayConditionId { get; set; }
        public string SellerName { get; set; }
        public string Frecuency { get; set; }
        public int SellerId { get; set; }
        public int InvoiceNumber { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime DocDate { get; set; }
        public decimal DocTotal { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Tax { get; set; }
        public decimal DiscountsTotal { get; set; }
        public decimal Balance { get; set; }
        public decimal PaidToDate { get; set; }
        public decimal UnexpiredBalance { get; set; }
        public decimal BalanceDue { get; set; }
        public decimal BalanceAt30Days { get; set; }
        public decimal BalanceFrom31To60Days { get; set; }
        public decimal BalanceFrom61To90Days { get; set; }
        public decimal BalanceFrom91To120Days { get; set; }
        public decimal BalanceMoreThan120Days { get; set; }
        public int DaysExpired { get; set; }
    }
}
