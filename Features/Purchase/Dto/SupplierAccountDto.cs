using System;

namespace Pos.WebApi.Features.Purchase.Dto
{
    public class SupplierAccountDto
    {
        public int SupplierId { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string PayConditionName { get; set; }
        public int InvoiceNumber { get; set; }
        public DateTime DueDate { get; set; }
        public decimal Balance { get; set; }
        public decimal UnexpiredBalance { get; set; }
        public decimal BalanceDue               { get; set; }
        public decimal BalanceAt30Days          { get; set; }
        public decimal BalanceFrom31To60Days    { get; set; }
        public decimal BalanceFrom61To90Days    { get; set; }
        public decimal BalanceFrom91To120Days   { get; set; }
        public decimal BalanceMoreThan120Days   { get; set; }
        public int DaysExpired { get; set; }
    }
}
