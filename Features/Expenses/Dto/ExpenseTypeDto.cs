using Pos.WebApi.Features.Expenses.Entities;

namespace Pos.WebApi.Features.Expenses.Dto
{
    public class ExpenseTypeDto: ExpenseType
    {
        public string CreatedByName { get; set; }
    }
}
