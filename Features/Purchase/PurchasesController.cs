using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pos.WebApi.Features.Purchase.Entities;
using Pos.WebApi.Features.Purchase.Services;
using System;

namespace Pos.WebApi.Features.Purchase
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PurchasesController : ControllerBase
    {
        private readonly PurchaseServices _purchaseServices;

        public PurchasesController(PurchaseServices purchaseServices)
        {
            _purchaseServices = purchaseServices;
        }

        [HttpGet("GetOrderPurchase")]
        public IActionResult GetOrderPurchase()
        {
            try
            {
                var result = _purchaseServices.GetOrderPurchase();
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpGet("GetOrderPurchaseActive")]
        public IActionResult GetOrderPurchaseActive()
        {
            try
            {
                var result = _purchaseServices.GetOrderPurchaseActive();
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }
        [Authorize]
        [HttpGet("GetOrderPurchaseByDate/{From}/{To}")]
        public IActionResult GetByDate(DateTime From, DateTime To)
        {
            try
            {
                var incomes = _purchaseServices.GetOrderPurchaseByDate(From, To);
                return Ok(incomes);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("GetOrderPurchaseById/{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var consolidator = _purchaseServices.GetOrderPurchaseById(id);
                return Ok(consolidator);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("AddOrderPurchase")]
        public IActionResult AddOrderPurchase(OrderPurchase request)
        {
            try
            {
                var result = _purchaseServices.AddOrderPurchase(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpPut("UpdateOrderPurchase")]
        public IActionResult UpdateOrderPurchase(OrderPurchase request)
        {
            try
            {
                var result = _purchaseServices.EditOrderPurchase(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }
        [HttpPut("CanceledOrderPurchase{docId}")]
        public IActionResult CanceledOrderPurchase(int docId)
        {
            try
            {
                var result = _purchaseServices.CanceledOrderPurchase(docId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        //Invoice
        [HttpGet("GetInvoicePurchase")]
        public IActionResult GetInvoicePurchase()
        {
            try
            {
                var result = _purchaseServices.GetInvoicePurchase();
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }
        [HttpGet("GetInvoicePurchaseActive")]
        public IActionResult GetInvoicePurchaseActive()
        {
            try
            {
                var result = _purchaseServices.GetInvoicePurchaseActive();
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpGet("GetInvoicePurchaseActiveSupplier{idSupplier}")]
        public IActionResult GetInvoicePurchaseActiveSupplier(int idSupplier)
        {
            try
            {
                var result = _purchaseServices.GetInvoicePurchaseActiveSupplier(idSupplier);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }
        [Authorize]
        [HttpGet("GetInvoicePurchaseByDate/{From}/{To}")]
        public IActionResult GetInvoiceByDate(DateTime From, DateTime To)
        {
            try
            {
                var incomes = _purchaseServices.GetInvoicePurchaseByDate(From, To);
                return Ok(incomes);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("GetInvoicePurchaseById/{id}")]
        public IActionResult GetInvoiceById(int id)
        {
            try
            {
                var consolidator = _purchaseServices.GetInvoicePurchaseById(id);
                return Ok(consolidator);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("AddInvoicePurchase")]
        public IActionResult AddInvoicePurchase(InvoicePurchase request)
        {
            try
            {
                var result = _purchaseServices.AddInvoicePurchase(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpPut("UpdateInvoicePurchase")]
        public IActionResult UpdateInvoicePurchase(InvoicePurchase request)
        {
            try
            {
                var result = _purchaseServices.EditInvoicePurchase(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }
        [HttpPut("CanceledInvoicePurchase{docId}")]
        public IActionResult CanceledInvoicePurchase(int docId)
        {
            try
            {
                var result = _purchaseServices.CanceledInvoicePurchase(docId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpGet("GetSupplierAccountBalance")]
        public IActionResult GetSupplierAccountBalance()
        {
            try
            {
                var consolidator = _purchaseServices.GetAccountBalance();
                return Ok(consolidator);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
