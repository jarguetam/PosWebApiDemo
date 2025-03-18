using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pos.WebApi.Features.Expenses.Entities;
using Pos.WebApi.Features.Expenses.Services;
using Pos.WebApi.Features.Liquidations.Dto;
using Pos.WebApi.Features.Liquidations.Entities;
using Pos.WebApi.Features.Liquidations.Services;
using System;

namespace Pos.WebApi.Features.Liquidations
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LiquidationController : ControllerBase
    {
        private readonly LiquidationServices _liquidationServices;

        public LiquidationController(LiquidationServices liquidationServices)
        {
            _liquidationServices = liquidationServices;
        }

        [HttpGet("GetLiquidationByDate/{From}/{To}")]
        public IActionResult GetLiquidationByDate(DateTime From, DateTime To)
        {
            try
            {
                var result = _liquidationServices.GetLiquidationByDate(From, To);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpGet("GetLiquidationDetail/{From}/{To}/{sellerId}")]
        public IActionResult GetLiquidationDetail(DateTime From, DateTime To, int sellerId)
        {
            try
            {
                var result = _liquidationServices.GetLiquidationsBySellerAndDate(From, To, sellerId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpPost("AddLiquidation")]
        public IActionResult AddExpense([FromBody] LiquidationDto request)
        {
            try
            {
                var result = _liquidationServices.AddLiquidation(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpPut("UpdateLiquidation")]
        public IActionResult EditLiquidation([FromBody] LiquidationDto request)
        {
            try
            {
                var result = _liquidationServices.EditLiquidation(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpPut("CancelLiquidation")]
        public IActionResult CancelLiquidation([FromBody] LiquidationDto request)
        {
            try
            {
                var result = _liquidationServices.CancelLiquidation(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpGet("GetLiquidationSellerResum/{sellerId}/{date}")]
        public IActionResult GetLiquidationSellerResum(int sellerId, DateTime date)
        {
            try
            {
                var result = _liquidationServices.GetLiquidationSellerResums(sellerId, date);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpGet("GetMoneyBill")]
        public IActionResult GetMoneyBill()
        {
            try
            {
                var result = _liquidationServices.GetMoneyBill();
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpPost("AddMoneyLiquidation")]
        public IActionResult AddMoneyLiquidation([FromBody] MoneyLiquidation request)
        {
            try
            {
                var result = _liquidationServices.AddMoneyLiquidation(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpPost("EditMoneyLiquidation")]
        public IActionResult EditMoneyLiquidation([FromBody] MoneyLiquidation request)
        {
            try
            {
                var result = _liquidationServices.UpdateMoneyLiquidation(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpGet("GetLiquidationMoney/{From}/{To}")]
        public IActionResult GetLiquidationMoney(DateTime From, DateTime To)
        {
            try
            {
                var result = _liquidationServices.GetMoneyLiquidationByDate(From, To);
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
