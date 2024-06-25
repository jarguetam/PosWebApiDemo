using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pos.WebApi.Features.PurchasePayment.Entitie;
using Pos.WebApi.Features.PurchasePayment.Service;
using System;

namespace Pos.WebApi.Features.PurchasePayment
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PurchasePaymentController : ControllerBase
    {
        private readonly PaymentPurchaseServices _paymentServices;

        public PurchasePaymentController(PaymentPurchaseServices services)
        {
            _paymentServices = services;
        }

        [HttpGet("GetPaymentPurchase")]
        public IActionResult GetPaymentPurchase()
        {
            try
            {
                var result = _paymentServices.GetPaymentPurchase();
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpGet("GetPaymentPurchaseActive")]
        public IActionResult GetPaymentPurchaseActive()
        {
            try
            {
                var result = _paymentServices.GetPaymentPurchaseActive();
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }
        [Authorize]
        [HttpGet("GetPaymentPurchaseByDate/{From}/{To}")]
        public IActionResult GetByDate(DateTime From, DateTime To)
        {
            try
            {
                var incomes = _paymentServices.GetPaymentPurchaseByDate(From, To);
                return Ok(incomes);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("GetPaymentPurchaseById/{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var consolidator = _paymentServices.GetPaymentPurchaseById(id);
                return Ok(consolidator);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("AddPaymentPurchase")]
        public IActionResult AddPaymentPurchase(PaymentPurchase request)
        {
            try
            {
                var result = _paymentServices.AddPaymentPurchase(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpPut("UpdatePaymentPurchase")]
        public IActionResult UpdatePaymentPurchase(PaymentPurchase request)
        {
            try
            {
                var result = "";//_paymentServices.EditPaymentPurchase(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }
        [HttpPut("CanceledPaymentPurchase{docId}")]
        public IActionResult CanceledPaymentPurchase(int docId)
        {
            try
            {
                var result = _paymentServices.CanceledPaymentPurchase(docId);
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
