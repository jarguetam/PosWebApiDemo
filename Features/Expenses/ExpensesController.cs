using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pos.WebApi.Features.Expenses.Entities;
using Pos.WebApi.Features.Expenses.Services;
using Pos.WebApi.Features.Items.Entities;
using Pos.WebApi.Features.Items.Services;
using Pos.WebApi.Features.Purchase.Entities;
using Pos.WebApi.Features.Purchase.Services;
using System;

namespace Pos.WebApi.Features.Expenses
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ExpensesController : ControllerBase
    {
        private readonly ExpenseServices _expenseServices;

        public ExpensesController(ExpenseServices expenseServices)
        {
            _expenseServices = expenseServices;
        }

        [HttpGet("ExpenseType")]
        public IActionResult GetExpenseType() 
        {
            try
            {
                var result = _expenseServices.GetExpenseTypes();
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpPost("AddExpenseType")]
        public IActionResult AddExpenseType([FromBody] ExpenseType request)
        {
            try
            {
                var result = _expenseServices.AddExpenseType(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpPut("EditExpenseType")]
        public IActionResult EditExpenseType([FromBody] ExpenseType request)
        {
            try
            {
                var result = _expenseServices.EditExpenseType(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpGet("GetExpenseByDate/{From}/{To}")]
        public IActionResult GetExpenseByDate(DateTime From, DateTime To)
        {
            try
            {
                var incomes = _expenseServices.GetExpenseByDate(From, To);
                return Ok(incomes);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("AddExpense")]
        public IActionResult AddExpense([FromBody] Expense request)
        {
            try
            {
                var result = _expenseServices.AddExpense(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpPut("UpdateExpense")]
        public IActionResult UpdateExpense(Expense request)
        {
            try
            {
                var result = _expenseServices.EditExpense(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpPut("CancelExpense")]
        public IActionResult CancelExpense(Expense request)
        {
            try
            {
                var result = _expenseServices.CancelExpense(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }
    }
}
