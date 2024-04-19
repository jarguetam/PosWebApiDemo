using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pos.WebApi.Features.Customers.Entities;
using Pos.WebApi.Features.Customers.Services;
using Pos.WebApi.Features.Items.Dto;
using Pos.WebApi.Features.Items.Entities;
using Pos.WebApi.Features.Sellers.Entities;
using System;
using System.Collections.Generic;

namespace Pos.WebApi.Features.Customers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CustomersController : ControllerBase
    {
        private readonly CustomerServices _services;

        public CustomersController(CustomerServices services)
        {
            _services = services;
        }

        [HttpGet("CustomerCategory")]
        public IActionResult GetCustomerCategory()
        {
            try
            {
                var result = _services.GetCustomerCategory();
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpGet("CustomerCategoryActive")]
        public IActionResult GetCustomerCategoryActive()
        {
            try
            {
                var result = _services.GetCustomerCategoryActive();
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpPost("AddCustomerCategory")]
        public IActionResult AddCustomerCategory([FromBody] CustomerCategory request)
        {
            try
            {
                var result = _services.AddCustomerCategory(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpPut("EditCustomerCategory")]
        public IActionResult EditCustomerCategory([FromBody] CustomerCategory request)
        {
            try
            {
                var result = _services.EditCustomerCategory(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }
        //Frequency
        [HttpGet("CustomerFrequency")]
        public IActionResult GetFrequency()
        {
            try
            {
                var result = _services.GetCustomerFrequency();
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpPost("AddCustomerFrequency")]
        public IActionResult AddCustomerFrequency([FromBody] CustomerFrequency request)
        {
            try
            {
                var result = _services.AddCustomerFrequency(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpPut("EditCustomerFrequency")]
        public IActionResult EditCustomerFrequency([FromBody] CustomerFrequency request)
        {
            try
            {
                var result = _services.EditCustomerFrequency(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }
        //Zone
        [HttpGet("CustomerZone")]
        public IActionResult GetCustomerZone()
        {
            try
            {
                var result = _services.GetCustomerZone();
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpPost("AddCustomerZone")]
        public IActionResult AddCustomerZone([FromBody] CustomerZone request)
        {
            try
            {
                var result = _services.AddCustomerZone(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpPut("EditCustomerZone")]
        public IActionResult EditCustomerZone([FromBody] CustomerZone request)
        {
            try
            {
                var result = _services.EditCustomerZone(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }
        
        //Pricelist

        [HttpGet("PriceList")]
        public IActionResult GetPriceList()
        {
            try
            {
                var result = _services.GetPriceList();
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpGet("PriceListActive")]
        public IActionResult GetPriceListActive()
        {
            try
            {
                var result = _services.GetPriceListActive();
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpGet("PriceListDetail")]
        public IActionResult GetPriceListDetail(int idPriceList)
        {
            try
            {
                var result = _services.GetPriceListDetail(idPriceList);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }
        [HttpPut("UpdatePriceListDetail")]
        public IActionResult UpdatePriceListDetail(List<PriceListDetail> request)
        {
            try
            {
                var result = _services.EditPriceListDetail(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }
        
        [HttpGet("PriceSpecialCustomerDetail{idPriceList}/{customerId}")]
        public IActionResult GetPriceSpecialCustomerDetail(int idPriceList, int customerId)
        {
            try
            {
                var result = _services.GetPriceSpecialListDetail(idPriceList, customerId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }
        
        [HttpGet("ItemPrice")]
        public IActionResult GetItemPrice()
        {
            try
            {
                var result = _services.GetItemPrices();
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }
        [HttpGet("ItemPriceCustomer{idPriceList}/{customerId}")]
        public IActionResult GetItemPriceCustomer(int idPriceList, int customerId)
        {
            try
            {
                var result = _services.GetPricesCustomer(idPriceList, customerId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpPost("AddPriceList")]
        public IActionResult AddPriceList([FromBody] PriceList request)
        {
            try
            {
                var result = _services.AddPriceList(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }
        [HttpPost("AddSpecialPriceCustomer")]
        public IActionResult AddSpecialPriceCustomer([FromBody] List<PriceSpecialCustomerDetail> request)
        {
            try
            {
                var result = _services.AddSpecialPriceCustomer(request);
                List<string> result2 = new List<string>();
                result2.Add(result);
                return Ok(result2);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpPut("EditPriceList")]
        public IActionResult EditPriceList([FromBody] PriceList request)
        {
            try
            {
                var result = _services.EditPriceList(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpGet("Customer")]
        public IActionResult GetCustomer()
        {
            try
            {
                var result = _services.GetCustomer();
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpGet("CustomerActive")]
        public IActionResult GetCustomerActive()
        {
            try
            {
                var result = _services.GetCustomerActive();
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpPost("AddCustomer")]
        public IActionResult AddCustomer([FromBody] Customer request)
        {
            try
            {
                var result = _services.AddCustomer(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje =
                    ex.InnerException != null ? ex.InnerException.Message.Contains("RTN") ? "RTN Duplicado" : ex.InnerException.Message.Contains("CustomerCode")? "Codigo se duplica, contacte a su administrador": ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpPut("EditCustomer")]
        public IActionResult EditCustomer([FromBody] Customer request)
        {
            try
            {
                var result = _services.EditCustomer(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje =
                    ex.InnerException != null ? ex.InnerException.Message.Contains("RTN") ? "RTN Duplicado" : ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

    }
}
