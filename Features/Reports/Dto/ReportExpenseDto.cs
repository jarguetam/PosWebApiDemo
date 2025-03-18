namespace Pos.WebApi.Features.Reports.Dto
{
    public class ReportExpenseDto
    {
        public int ExpenseId { get; set; }
        public string ExpenseName { get; set;}
        public string ExpenseType { get; set;}
        public decimal ExpenseAmount { get; set;}
    }
}
