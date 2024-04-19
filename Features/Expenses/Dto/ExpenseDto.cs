using Pos.WebApi.Features.Expenses.Entities;
using Pos.WebApi.Features.Purchase.Dto;
using System.Collections.Generic;

namespace Pos.WebApi.Features.Expenses.Dto
{
    public class ExpenseDto : Expense
    {
        public string CreateByName { get; set; }
        public string SellerName { get; set; }
        public new List<ExpenseDetailDto> Detail
        {
            get; set;
        }
    }
}
