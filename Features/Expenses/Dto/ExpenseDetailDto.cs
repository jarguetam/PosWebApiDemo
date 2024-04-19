using Pos.WebApi.Features.Expenses.Entities;

namespace Pos.WebApi.Features.Expenses.Dto
{
    public class ExpenseDetailDto: ExpenseDetail
    {
        public string ExpenseTypeName { get; set; }
    }
}
