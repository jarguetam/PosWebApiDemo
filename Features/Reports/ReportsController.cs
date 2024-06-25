using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pos.WebApi.Features.Dashboard.Dto;
using Pos.WebApi.Features.Dashboard;
using System.Collections.Generic;
using System;
using Pos.WebApi.Features.Reports.Services;
using Microsoft.AspNetCore.Authorization;

namespace Pos.WebApi.Features.Reports
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class ReportsController : ControllerBase
    {
        private readonly ReportServices _service;

        public ReportsController(ReportServices reportService)
        {
            _service = reportService;
        }


        [HttpGet("GetInventoryReport")]
        public IActionResult GetInventoryReport()
        {
            try
            {
                var result = _service.GetInventoryReports();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("GetInventoryWareHouseReport")]
        public IActionResult GetInventoryWareHouseReport()
        {
            try
            {
                var result = _service.GetInventoryWarehouseReports();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("GetSalesReport{from}/{to}")]
        public IActionResult GetSalesReport(DateTime from, DateTime to)
        {
            var ms = _service.GenerateReportSalesDate(from, to);
            return new FileStreamResult(ms, "application/pdf");            
        }

        [HttpGet("GetCXCReport{idseller}")]
        public IActionResult GetCXCReport(int idseller)
        {
            var ms = _service.GenerateReportCxc(idseller);
            return new FileStreamResult(ms, "application/pdf");
        }

    }
}
