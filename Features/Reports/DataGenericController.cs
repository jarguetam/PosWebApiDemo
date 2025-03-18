using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pos.WebApi.Features.Reports.Services;
using System.Threading.Tasks;

namespace Pos.WebApi.Features.Reports
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DataGenericController : ControllerBase
    {
        private readonly GenericDataService _dataService;

        public DataGenericController(GenericDataService dataService)
        {
            _dataService = dataService;
        }

        [HttpGet("vista")]
        public async Task<IActionResult> GetDataFromView(string vista)
        {
            var result = await _dataService.ExecuteQueryAsync(@$"SELECT * FROM {vista}");
            return Ok(result);
        }

        [HttpGet("procedimiento")]
        public async Task<IActionResult> GetDataFromStoredProcedure(string procedimiento)
        {
            var parameters = new { AlmacenOrigen = "1", AlmacenDestino = "4" };
            var result = await _dataService.ExecuteStoredProcedureAsync(procedimiento, parameters);
            return Ok(result);
        }

        [HttpGet("GetReports")]
        public async Task<IActionResult> GetReports()
        {
            
            var result =  _dataService.GetReports();
            return Ok(result);
        }
    }
}
