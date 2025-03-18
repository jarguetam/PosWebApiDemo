using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pos.WebApi.Features.InventoryTransactions.Dto;
using Pos.WebApi.Features.InventoryTransactions.Entities;
using Pos.WebApi.Features.InventoryTransactions.Services;
using Pos.WebApi.Features.Items.Entities;
using Pos.WebApi.Features.Items.Services;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace Pos.WebApi.Features.InventoryTransactions
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InventoryTransactionController : ControllerBase
    {
        InventoryTransactionServices _service;

        public InventoryTransactionController(InventoryTransactionServices service)
        {
            _service = service;
        }
        [HttpGet("InventoryTransactionType")]
        public IActionResult GetInventoryTransactionType()
        {
            try
            {
                var result = _service.GetInventoryTransactionType();
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpGet("InventoryTransactionTypeBy{type}")]
        public IActionResult GetInventoryTransactionTypeActive(string type)
        {
            try
            {
                var result = _service.GetInventoryTransactionTypeBy(type);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpPost("AddInventoryTransactionType")]
        public IActionResult AddInventoryTransactionType([FromBody] InventoryTransactionType request)
        {
            try
            {
                var result = _service.AddInventoryTransactionType(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpPut("EditInventoryTransactionType")]
        public IActionResult EditInventoryTransactionType([FromBody] InventoryTransactionType request)
        {
            try
            {
                var result = _service.EditInventoryTransactionType(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpGet("GetInventoryEntry")]
        public IActionResult GetInventoryEntry()
        {
            try
            {
                var result = _service.GetInventoryEntry();
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }
        [Authorize]
        [HttpGet("GetInventoryEntryByDate/{From}/{To}")]
        public IActionResult GetByDate(DateTime From, DateTime To)
        {
            try
            {
                var incomes = _service.GetEntryByDate(From, To);
                return Ok(incomes);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("GetInventoryEntryById/{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var consolidator = _service.GetEntryById(id);
                return Ok(consolidator);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("AddInventoryEntry")]
        public IActionResult AddInventoryEntry(InventoryEntry request)
        {
            try
            {
                var result = _service.AddInventoryEntry(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        //OutPut
        [HttpGet("GetInventoryOutPut")]
        public IActionResult GetInventoryOutPut()
        {
            try
            {
                var result = _service.GetInventoryOutPut();
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }
        [Authorize]
        [HttpGet("GetInventoryOutPutByDate/{From}/{To}")]
        public IActionResult GetByDateOutPut(DateTime From, DateTime To)
        {
            try
            {
                var incomes = _service.GetOutPutByDate(From, To);
                return Ok(incomes);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("GetInventoryOutPutById/{id}")]
        public IActionResult GetOutPutById(int id)
        {
            try
            {
                var consolidator = _service.GetOutPutById(id);
                return Ok(consolidator);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("AddInventoryOutPut")]
        public IActionResult AddInventoryOutPut(InventoryOutPut request)
        {
            try
            {
                var result = _service.AddInventoryOutPut(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        //Transfer
        [HttpGet("GetInventoryTransfer")]
        public IActionResult GetInventoryTransfer()
        {
            try
            {
                var result = _service.GetInventoryTransfer();
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }
   
        [HttpGet("GetInventoryTransferByDate/{From}/{To}")]
        public IActionResult GetByDateTransfer(DateTime From, DateTime To)
        {
            try
            {
                var incomes = _service.GetTransferByDate(From, To);
                return Ok(incomes);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("GetInventoryTransferById/{id}")]
        public IActionResult GetTransferById(int id)
        {
            try
            {
                var response = _service.GetTransferById(id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("AddInventoryTransfer")]
        public IActionResult AddInventoryTransfer(InventoryTransfer request)
        {
            try
            {
                var result = _service.AddInventoryTransfer(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        //Cost Revaluation
        [HttpGet("GetCostRevaluationByDate/{From}/{To}")]
        public IActionResult GetCostRevaluationByDate(DateTime From, DateTime To)
        {
            try
            {
                var incomes = _service.GetRevaluationCostByDate(From, To);
                return Ok(incomes);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("AddCostRevaluation")]
        public IActionResult AddCostRevaluation(List<CostRevaluation> request)
        {
            try
            {
                var result =new List<CostRevaluationDto>();
                request.ForEach(x => result=  _service.AddRevaluationCost(x));
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }
        //Request Transfer
        [HttpGet("GetItemsToTransfer")]
        public async Task<IActionResult> GetItemsToTransferAsync(int almacenOrigen, int almacenDestino)
        {
            try
            {
                var result =  await _service.GetItemsToTransfer(almacenOrigen, almacenDestino);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpGet("GetInventoryRequestTransfer")]
        public IActionResult GetInventoryRequestTransfer()
        {
            try
            {
                var result = _service.GetInventoryRequestTransfer();
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpGet("GetInventoryRequestTransferToComplete")]
        public IActionResult GetInventoryRequestTransferToComplete()
        {
            try
            {
                var result = _service.GetRequestTransferToComplete();
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpGet("GetInventoryRequestTransferByDate/{From}/{To}/{userId}")]
        public IActionResult GetByDateRequestTransfer(DateTime From, DateTime To, int userId)
        {
            try
            {
                var incomes = _service.GetRequestTransferByDate(From, To, userId);
                return Ok(incomes);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("GetInventoryRequestTransferById/{id}")]
        public IActionResult GetRequestTransferById(int id)
        {
            try
            {
                var response = _service.GetRequestTransferById(id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("AddInventoryRequestTransfer")]
        public IActionResult AddInventoryRequestTransfer(InventoryRequestTransfer request)
        {
            try
            {
                var result = _service.AddInventoryRequestTransfer(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpPut("UpdateInventoryRequestTransfer")]
        public IActionResult UpdateInventoryRequestTransfer(InventoryRequestTransfer request)
        {
            try
            {
                var result = _service.UpdateInventoryRequestTransfer(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpGet("GetInventoryReturn/{from}/{to}")]
        public IActionResult GetInventoryReturn(DateTime from, DateTime to)
        {
            try
            {
                var response = _service.GetInventoryReturnByDate(from, to);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("AddInventoryReturn")]
        public IActionResult AddInventoryReturn(InventoryReturn request)
        {
            try
            {
                var response = _service.AddInventoryReturn(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("UpdateInventoryReturn")]
        public IActionResult UpdateInventoryReturn(InventoryReturn request)
        {
            try
            {
                var response = _service.UpdateInventoryReturn(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("CompleteInventoryReturn")]
        public IActionResult CompleteInventoryReturn(InventoryReturn request)
        {
            try
            {
                var response = _service.CompleteInventoryReturn(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("GetInventoryReturnResumen/{date}/{whscode}")]
        public IActionResult GetInventoryReturn(DateTime date, int whscode)
        {
            try
            {
                var response = _service.GetResumenReturn(date, whscode);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


    }
}
