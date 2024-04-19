using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pos.WebApi.Features.Items.Entities;
using Pos.WebApi.Features.Items.Services;
using Pos.WebApi.Features.Sellers.Entities;
using Pos.WebApi.Features.Sellers.Services;
using System;

namespace Pos.WebApi.Features.Sellers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SellersController : ControllerBase
    {
        private readonly SellerServices _services;

        public SellersController(SellerServices services)
        {
            _services = services;
        }

        [HttpGet("SellerRegion")]
        public IActionResult GetSellerRegion()
        {
            try
            {
                var result = _services.GetSellerRegion();
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpGet("SellerRegionActive")]
        public IActionResult GetSellerRegionActive()
        {
            try
            {
                var result = _services.GetSellerRegionActive();
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpPost("AddSellerRegion")]
        public IActionResult AddSellerRegion([FromBody] SellerRegion request)
        {
            try
            {
                var result = _services.AddSellerRegion(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }


        [HttpPut("EditSellerRegion")]
        public IActionResult EditSellerRegion([FromBody] SellerRegion request)
        {
            try
            {
                var result = _services.EditSellerRegion(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpGet("Seller")]
        public IActionResult GetSeller()
        {
            try
            {
                var result = _services.GetSeller();
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpPost("AddSeller")]
        public IActionResult AddSeller([FromBody] Seller request)
        {
            try
            {
                var result = _services.AddSeller(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }


        [HttpPut("EditSeller")]
        public IActionResult EditSeller([FromBody] Seller request)
        {
            try
            {
                var result = _services.EditSeller(request);
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
