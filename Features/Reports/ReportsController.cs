using Microsoft.AspNetCore.Mvc;
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

        [HttpGet("GetCXCReport{idseller}/{onlyOverdue}")]
        public IActionResult GetCXCReport(int idseller, bool onlyOverdue)
        {
            var ms = _service.GenerateReportCxc(idseller, onlyOverdue);
            return new FileStreamResult(ms, "application/pdf");
        }

        [HttpGet("GetReportInventoryCategory{whsCode}")]
        public IActionResult GetReportInventoryCategory(int whsCode)
        {
            var ms = _service.GenerateReportInventoryPdf(whsCode);
            return new FileStreamResult(ms, "application/pdf");
        }

        [HttpGet("GetReportMargen{from}/{to}/{sellerId}")]
        public IActionResult GetReportorMargen(DateTime from, DateTime to, int sellerId)
        {
            var ms = _service.GenerateReporteSalesMargen(from, to, sellerId);
            return new FileStreamResult(ms, "application/pdf");
        }

        [HttpGet("GetReportExpense{from}/{to}/{sellerId}")]
        public IActionResult GetReportExpense(DateTime from, DateTime to, int sellerId)
        {
            var ms = _service.GenerateReportExpense(from, to, sellerId);
            return new FileStreamResult(ms, "application/pdf");
        }

    }
}
