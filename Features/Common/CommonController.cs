using System;
using Pos.WebApi.Features.Common.Entities;
using Pos.WebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Pos.WebApi.Features.Customers.Entities;

namespace Pos.WebApi.Features.Common
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommonController : ControllerBase
    {
        private readonly CommonService _commonService;
        private readonly BPJornalServices _bpJornalService;
        public CommonController(BPJornalServices bpJornalService, CommonService commonService)
        {
            _commonService = commonService;
            _bpJornalService = bpJornalService; 
        }      

        [Authorize]
        [HttpPost("UploadFile")]
        public IActionResult UploadFile(IFormFile File)
        {
            try
            {
                var result = _commonService.UploadFile(File);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        //[Authorize]
        [HttpPost("FileInfo")]
        public IActionResult FileInfo(int id)
        {
            try
            {
                var result = _commonService.FileInfo(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        //[Authorize]
        [HttpGet("DownloadFile")]
        public IActionResult DownloadFile([FromQuery(Name = "Path")] string  Path)
        {
            try
            {
                var result = _commonService.DownloadFile(Path);
                var rt = PhysicalFile(result.Path, result.MimeType);
                return rt;

            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        //[Authorize]
        [HttpGet("GetCompanyInfo")]
        public IActionResult GetCompanyInfo()
        {
            try
            {
                var result = _commonService.GetCompanyInfo();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [Authorize]
        [HttpPost("AddCompanyInfo")]
        public IActionResult AddCompanyInfo([FromBody] CompanyInfo CompanyInfo)
        {
            try
            {
                var result = _commonService.AddCompany(CompanyInfo);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [Authorize]
        [HttpPut("EditCompanyInfo")]
        public IActionResult EditCompanyInfo([FromBody] CompanyInfo CompanyInfo)
        {
            try
            {
                var result = _commonService.EditCompanyInfo(CompanyInfo);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpGet("PayCondition")]
        public IActionResult GetPayCondition()
        {
            try
            {
                var result = _commonService.GetCPayCondition();
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpGet("PayConditionActive")]
        public IActionResult GetCPayConditionActive()
        {
            try
            {
                var result = _commonService.GetCPayConditionActive();
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpPost("AddPayCondition")]
        public IActionResult AddPayCondition([FromBody] PayCondition request)
        {
            try
            {
                var result = _commonService.AddPayCondition(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }


        [HttpPut("EditPayCondition")]
        public IActionResult EditPayCondition([FromBody] PayCondition request)
        {
            try
            {
                var result = _commonService.EditPayCondition(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }
        #region SAR
        [Authorize]
        [HttpGet("GetCorrelative")]
        public IActionResult GetCorrelative()
        {
            try
            {
                var result = _commonService.GetCorrelative();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("GetCorrelativeInvoice")]
        public IActionResult GetCorrelativeInvoice()
        {
            try
            {
                var result = _commonService.GetCorrelativeInvoice();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("GetCorrelativeInvoiceById{id}")]
        public IActionResult GetCorrelativeInvoiceById(int id)
        {
            try
            {
                var result = _commonService.GetCorrelativeInvoiceById(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("AddCorrelative")]
        public IActionResult AddCorrelative([FromBody] SarCorrelative correlative)
        {
            try
            {
                var result = _commonService.AddCorrelative(correlative);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("EditCorrelative")]
        public IActionResult EditCorrelative([FromBody] SarCorrelative correlative)
        {
            try
            {
                var result = _commonService.EditCorrelative(correlative);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("GetPointSale")]
        public IActionResult GetPointSale()
        {
            try
            {
                var result = _commonService.GetPointSale();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    
        [Authorize]
        [HttpPost("AddPointSale")]
        public IActionResult AddPointSale([FromBody] SarPointSale correlative)
        {
            try
            {
                var result = _commonService.AddPointSale(correlative);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("EditPointSale")]
        public IActionResult EditPointSale([FromBody] SarPointSale correlative)
        {
            try
            {
                var result = _commonService.EditPointSale(correlative);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("GetBranch")]
        public IActionResult GetBranch()
        {
            try
            {
                var result = _commonService.GetBranch();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("GetTypeDocument")]
        public IActionResult GetTypeDocument()
        {
            try
            {
                var result = _commonService.GetTypeDocument();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        #endregion
        [Authorize]
        [HttpGet("GetJornalBp{bpId}/{type}")]
        public IActionResult GetJornalBp(int bpId, string type)
        {
            try
            {
                var result = _bpJornalService.GetJournal(bpId, type);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
