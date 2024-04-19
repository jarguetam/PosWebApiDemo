using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pos.WebApi.Features.InventoryTransactions.Services;
using System;

namespace Pos.WebApi.Features.InventoryTransactions
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ItemJournalController : ControllerBase
    {
        private readonly ItemJournalServices _services;

        public ItemJournalController(ItemJournalServices services)
        {
            _services = services;
        }

        [HttpGet("GetItemJornal{itemId}")]
        public IActionResult GetInventoryEntry(int itemId)
        {
            try
            {
                var result = _services.GetJornalItems(itemId);
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
