using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pos.WebApi.Features.Suppliers.Entities;
using Pos.WebApi.Features.Suppliers.Services;
using System;

namespace Pos.WebApi.Features.Suppliers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SupplierController : ControllerBase
    {
        private readonly SupplierServices _services;

        public SupplierController(SupplierServices services)
        {
            _services = services;
        }

        [HttpGet("SupplierCategory")]
        public IActionResult GetSupplierCategory()
        {
            try
            {
                var result = _services.GetSupplierCategory();
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpGet("SupplierCategoryActive")]
        public IActionResult GetSupplierCategoryActive()
        {
            try
            {
                var result = _services.GetSupplierCategoryActive();
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpPost("AddSupplierCategory")]
        public IActionResult AddSupplierCategory([FromBody] SupplierCategory request)
        {
            try
            {
                var result = _services.AddSupplierCategory(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpPut("EditSupplierCategory")]
        public IActionResult EditSupplierCategory([FromBody] SupplierCategory request)
        {
            try
            {
                var result = _services.EditSupplierCategory(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        //Suppliers
        [HttpGet("Supplier")]
        public IActionResult GetSupplier()
        {
            try
            {
                var result = _services.GetSupplier();
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpGet("SupplierActive")]
        public IActionResult GetSupplierActive()
        {
            try
            {
                var result = _services.GetSupplierActive();
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpPost("AddSupplier")]
        public IActionResult AddSupplier([FromBody] Supplier request)
        {
            try
            {
                var result = _services.AddSupplier(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje =
                    ex.InnerException != null ? ex.InnerException.Message.Contains("RTN") ? "RTN Duplicado" : ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }


        [HttpPut("EditSupplier")]
        public IActionResult EditSupplier([FromBody] Supplier request)
        {
            try
            {
                var result = _services.EditSupplier(request);
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
