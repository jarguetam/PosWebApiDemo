using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pos.WebApi.Features.Purchase.Services;
using Pos.WebApi.Features.Sales.Dto;
using Pos.WebApi.Features.Sales.Entities;
using Pos.WebApi.Features.Sales.Services;
using System;

namespace Pos.WebApi.Features.Sales
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SalesController : ControllerBase
    {
        private readonly SalesServices _saleServices;

        public SalesController(SalesServices saleServices)
        {
            _saleServices = saleServices;
        }

        [HttpGet("GetOrderSale")]
        public IActionResult GetOrderSale()
        {
            try
            {
                var result = _saleServices.GetOrderSale();
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpGet("GetOrderSaleActive")]
        public IActionResult GetOrderSaleActive()
        {
            try
            {
                var result = _saleServices.GetOrderSaleActive();
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpGet("GetOrderSaleByDate/{From}/{To}")]
        public IActionResult GetByDate(DateTime From, DateTime To)
        {
            try
            {
                var incomes = _saleServices.GetOrderSaleByDate(From, To);
                return Ok(incomes);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("GetOrderSaleById/{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var consolidator = _saleServices.GetOrderSaleById(id);
                return Ok(consolidator);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("AddOrderSale")]
        public IActionResult AddOrderSale(OrderSale request)
        {
            try
            {
                var result = _saleServices.AddOrderSale(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpPut("UpdateOrderSale")]
        public IActionResult UpdateOrderSale(OrderSale request)
        {
            try
            {
                var result = _saleServices.EditOrderSale(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }
        [HttpPut("CanceledOrderSale{docId}")]
        public IActionResult CanceledOrderSale(int docId)
        {
            try
            {
                var result = _saleServices.CanceledOrderSale(docId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        //Invoice
        [HttpGet("GetInvoiceSale")]
        public IActionResult GetInvoiceSale()
        {
            try
            {
                var result = _saleServices.GetInvoiceSale();
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }
        [HttpGet("GetInvoiceSaleActive")]
        public IActionResult GetInvoiceSaleActive()
        {
            try
            {
                var result = _saleServices.GetInvoiceSaleActive();
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpGet("GetInvoiceSaleActiveCustomer{idCustomer}")]
        public IActionResult GetInvoiceSaleActiveCustomer(int idCustomer)
        {
            try
            {
                var result = _saleServices.GetInvoiceSaleActiveCustomer(idCustomer);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpGet("GetInvoiceSaleActiveSeller{idSeller}")]
        public IActionResult GetInvoiceSaleActiveSeller(int idSeller)
        {
            try
            {
                var result = _saleServices.GetInvoiceSaleActiveSeller(idSeller);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [Authorize]
        [HttpGet("GetInvoiceSaleByDate/{From}/{To}")]
        public IActionResult GetInvoiceByDate(DateTime From, DateTime To)
        {
            try
            {
                var incomes = _saleServices.GetInvoiceSaleByDate(From, To);
                return Ok(incomes);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("GetInvoiceSaleById/{id}")]
        public IActionResult GetInvoiceById(int id)
        {
            try
            {
                var consolidator = _saleServices.GetInvoiceSaleById(id);
                return Ok(consolidator);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("AddInvoiceSale")]
        public IActionResult AddInvoiceSale(InvoiceSaleDto request)
        {
            try
            {
                var result = _saleServices.AddInvoiceSale(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpPut("UpdateInvoiceSale")]
        public IActionResult UpdateInvoiceSale(InvoiceSale request)
        {
            try
            {
                var result = _saleServices.EditInvoiceSale(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }
        [HttpPut("CanceledInvoiceSale{docId}")]
        public IActionResult CanceledInvoiceSale(int docId)
        {
            try
            {
                var result = _saleServices.CanceledInvoiceSale(docId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpGet("GetCustomerAccountBalance")]
        public IActionResult GetSupplierAccountBalance()
        {
            try
            {
                var consolidator = _saleServices.GetCustomerAccountBalance();
                return Ok(consolidator);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


    }
}
