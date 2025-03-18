using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pos.WebApi.Features.SalesPayment.Entities;
using Pos.WebApi.Features.SalesPayment.Service;
using System;
using System.Threading.Tasks;

namespace Pos.WebApi.Features.SalesPayment
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SalesPaymentController : ControllerBase
    {
        private readonly PaymentSaleServices _paymentServices;

        public SalesPaymentController(PaymentSaleServices services)
        {
            _paymentServices = services;
        }

        [HttpGet("GetPaymentSales")]
        public IActionResult GetPaymentSales()
        {
            try
            {
                var result = _paymentServices.GetPaymentSale();
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpGet("GetPaymentSalesActive")]
        public IActionResult GetPaymentSalesActive()
        {
            try
            {
                var result = _paymentServices.GetPaymentSaleActive();
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }
        [Authorize]
        [HttpGet("GetPaymentSalesByDate/{From}/{To}")]
        public IActionResult GetByDate(DateTime From, DateTime To)
        {
            try
            {
                var incomes = _paymentServices.GetPaymentSaleByDate(From, To);
                return Ok(incomes);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("GetPaymentSalesById/{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var consolidator = _paymentServices.GetPaymentSaleById(id);
                return Ok(consolidator);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("AddPaymentSales")]
        public IActionResult AddPaymentSales(PaymentSale request)
        {
            try
            {
                var result = _paymentServices.AddPaymentSale(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;

                if (mensaje.Contains("Cannot insert duplicate key row") && mensaje.Contains("Pago_ya_sincronizada"))
                {
                    return BadRequest(new { message = "Error: Este pago ya existe en la base de datos. UUID" });
                }

                return BadRequest(new { message = mensaje });
            }
        }

        [HttpPut("UpdatePaymentSales")]
        public IActionResult UpdatePaymentSales(PaymentSale request)
        {
            try
            {
                var result = _paymentServices.EditPaymentSale(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }
        [HttpPut("CanceledPaymentSales{docId}")]
        public async Task<IActionResult> CanceledPaymentSales(int docId)
        {
            try
            {
                var result = await _paymentServices.CanceledPaymentSales(docId);
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
