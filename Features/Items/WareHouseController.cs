using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pos.WebApi.Features.Items.Services;
using Pos.WebApi.Features.Users.Entities;
using Pos.WebApi.Features.Users;
using System;
using Pos.WebApi.Features.Items.Entities;

namespace Pos.WebApi.Features.Items
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WareHouseController : ControllerBase
    {
        private readonly WareHouseServices _wareHouseServices;

        public WareHouseController(WareHouseServices wareHouseServices)
        {
            _wareHouseServices = wareHouseServices;
        }

        [HttpGet]
        public IActionResult GetWareHouse()
        {
            try
            {
                var result = _wareHouseServices.GetWareHouse();
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpGet("WareHouseActive")]
        public IActionResult GetWareHouseActive()
        {
            try
            {
                var result = _wareHouseServices.GetWareHouseActive();
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpPost]
        public IActionResult Add([FromBody] WareHouse request)
        {
            try
            {
                var result = _wareHouseServices.AddWareHouse(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }


        [HttpPut]
        public IActionResult Edit([FromBody] WareHouse request)
        {
            try
            {
                var result = _wareHouseServices.EditWareHouse(request);
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
