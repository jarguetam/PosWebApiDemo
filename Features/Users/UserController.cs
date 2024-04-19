using System;
using Pos.WebApi.Features.Users.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Pos.WebApi.Features.Users
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private UserService _userService;
        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var users = _userService.Get();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost]
        public IActionResult Add([FromBody] User user)
        {
            try
            {
                var result = _userService.Add(user);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [Authorize]
        [HttpPut]
        public IActionResult Edit([FromBody] User user)
        {
            try
            {
                var result = _userService.Edit(user);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("Themes")]
        public IActionResult GetThemes()
        {
            try
            {
                var themes = _userService.GetThemes();
                return Ok(themes);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
