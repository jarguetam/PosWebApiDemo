using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pos.WebApi.Features.Items.Entities;
using Pos.WebApi.Features.Items.Services;
using System;

namespace Pos.WebApi.Features.Items
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ItemController : ControllerBase
    {
        private readonly ItemServices _services;

        public ItemController(ItemServices itemServices)
        {
            _services = itemServices;
        }

        [HttpGet("ItemCategory")]
        public IActionResult GetItemCategory()
        {
            try
            {
                var result = _services.GetItemCategory();
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpGet("ItemCategoryActive")]
        public IActionResult GetItemCategoryActive()
        {
            try
            {
                var result = _services.GetItemCategoryActive();
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpPost("AddItemCategory")]
        public IActionResult AddItemCategory([FromBody] ItemCategory request)
        {
            try
            {
                var result = _services.AddItemCategory(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpPut("EditItemCategory")]
        public IActionResult EditItemCategory([FromBody] ItemCategory request)
        {
            try
            {
                var result = _services.EditItemCategory(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }
        //Family
        [HttpGet("ItemSubCategory")]
        public IActionResult GetItemSubCategory()
        {
            try
            {
                var result = _services.GetItemFamily();
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpGet("ItemSubCategoryActive")]
        public IActionResult GetItemSubCategoryActive()
        {
            try
            {
                var result = _services.GetItemFamilyActive();
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpPost("AddItemSubCategory")]
        public IActionResult AddItemSubCategory([FromBody] ItemFamily request)
        {
            try
            {
                var result = _services.AddItemFamily(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpPut("EditItemSubCategory")]
        public IActionResult EditItemSubCategory([FromBody] ItemFamily request)
        {
            try
            {
                var result = _services.EditItemFamily(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }
        //Medidas
        [HttpGet("UnitOfMeasure")]
        public IActionResult GetUnitOfMeasure()
        {
            try
            {
                var result = _services.GetUnitOfMeasure();
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpGet("UnitOfMeasureActive")]
        public IActionResult GetUnitOfMeasureActive()
        {
            try
            {
                var result = _services.GetUnitOfMeasureActive();
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpPost("AddUnitOfMeasure")]
        public IActionResult AddUnitOfMeasure([FromBody] UnitOfMeasure request)
        {
            try
            {
                var result = _services.AddUnitOfMeasure(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }


        [HttpPut("EditUnitOfMeasure")]
        public IActionResult EditUnitOfMeasure([FromBody] UnitOfMeasure request)
        {
            try
            {
                var result = _services.EditUnitOfMeasure(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpGet("Item")]
        public IActionResult GetItem()
        {
            try
            {
                var result = _services.GetItem();
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpGet("ItemActive")]
        public IActionResult GetItemActive()
        {
            try
            {
                var result = _services.GetItemActive();
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpGet("ItemActiveByBarcode{barcode}")]
        public IActionResult GetItemActiveByBarcode(string barcode)
        {
            try
            {
                var result = _services.GetItemByBarcode(barcode);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpGet("ItemStockWareHouse{whscode}")]
        public IActionResult GetItemStockWareHouse( int whscode)
        {
            try
            {
                var result = _services.GetItemWareHouseStock(whscode);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpGet("ItemStockWareHouseBarcode{whscode}/{barcode}")]
        public IActionResult GetItemStockWareHouseBarcode(int whscode, string barcode)
        {
            try
            {
                var result = _services.GetItemWareHouseStockBarCode(whscode, barcode);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpGet("ItemStockWareHousePrice{whscode}/{customerId}/{listPriceId}")]
        public IActionResult GetItemStockWareHousePrice(int whscode, int customerId, int listPriceId)
        {
            try
            {
                var result = _services.GetItemWareHouseStockPrice(whscode, customerId, listPriceId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpGet("AlltemStockWareHousePrice{whscode}")]
        public IActionResult GetAllItemStockWareHousePrice(int whscode)
        {
            try
            {
                var result = _services.GetAllItemWareHouseStockPrice(whscode);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpGet("ItemStockWareHousePriceBarcode{whscode}/{customerId}/{listPriceId}/{barCode}")]
        public IActionResult GetItemStockWareHousePriceBarCode(int whscode, int customerId, int listPriceId, string barCode)
        {
            try
            {
                var result = _services.GetItemWareHouseStockPriceBarCode(whscode, customerId, listPriceId, barCode);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpGet("ItemPurchase")]
        public IActionResult GetItemPurchase()
        {
            try
            {
                var result = _services.GetItemPurchase();
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpPost("AddItem")]
        public IActionResult AddItem([FromBody] Item request)
        {
            try
            {
                var result = _services.AddItem(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                var mensaje =
                    ex.InnerException != null ? ex.InnerException.Message.Contains("Codigo") ? "Codigo Duplicado, contactese con el administrador." : ex.InnerException.Message : ex.Message;
                return BadRequest(new { message = mensaje });
            }
        }

        [HttpPut("EditItem")]
        public IActionResult EditItem([FromBody] Item request)
        {
            try
            {
                var result = _services.EditItem(request);
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
