using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pos.WebApi.Features.Dashboard.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pos.WebApi.Features.Dashboard
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly DashboardServices _service;

        public DashboardController(DashboardServices dashboardServices)
        {
            _service = dashboardServices;
        }

        [HttpGet("GetDataDasboard{userId}")]
        public async Task<IActionResult> GetDataDashboard(int userId)
        {
            try
            {
                var tasks = new[]
                {
            new DasboardDataModel
            {
                Id = 1,
                Name = "Clientes",
                Data = await _service.GetCustomerActiveAsync()
            },
            new DasboardDataModel
            {
                Id = 2,
                Name = "Proveedores",
                Data = await _service.GetSupplierActiveAsync()
            },
            new DasboardDataModel
            {
                Id = 3,
                Name = "Articulos",
                Data = await _service.GetItemActiveAsync()
            },
            new DasboardDataModel
            {
                Id = 4,
                Name = "Almacenes",
                Data = await _service.GetWareHouseActiveAsync()
            },
            new DasboardDataModel
            {
                Id = 5,
                Name = "Ventas por dia",
                Data = await _service.GetIncomePerDayAsync(userId)
            },
            new DasboardDataModel
            {
                Id = 6,
                Name = "Pedidos por vendedor",
                Data = await _service.GetOrderBySellerAsync(userId)
            },
            new DasboardDataModel
            {
                Id = 7,
                Name = "Inventario por almacen",
                Data = await _service.GetStockWareHouseAsync()
            },
            new DasboardDataModel
            {
                Id = 8,
                Name = "Inventario por categoria",
                Data = await _service.GetStockTypeAsync()
            },
            new DasboardDataModel
            {
                Id = 9,
                Name = "Facturacion por mes",
                Data = await _service.GetInvoicePerMonthAsync(userId)
            },
            new DasboardDataModel
            {
                Id = 10,
                Name = "Rentabilidad por mes",
                Data = await _service.GetCostEffectivenessPerMonthAsync(userId)
            },
            new DasboardDataModel
            {
                Id = 11,
                Name = "Valor Cuentas por Cobrar",
                Data = await _service.GetCXCAsync(userId)
            },
            new DasboardDataModel
            {
                Id = 12,
                Name = "Top 5 articulos mas vendidos",
                Data = await _service.GetItemBestSellsAsync(userId)
            },
            new DasboardDataModel
            {
                Id = 13,
                Name = "Valor Cuentas por Pagar",
                Data = await _service.GetCXPAsync(userId)
            },
            new DasboardDataModel
            {
                Id = 14,
                Name = "Top 5 articulos mas comprados",
                Data = await _service.GetItemBestPurchaseAsync(userId)
            },
            new DasboardDataModel
            {
                Id = 15,
                Name = "Compras por dia",
                Data = await _service.GetIncomePurchasePerDayAsync(userId)
            },
            new DasboardDataModel
            {
                Id = 16,
                Name = "Compras por mes",
                Data = await _service.GetInvoicePurchasePerMonthAsync(userId)
            },
            new DasboardDataModel
            {
                Id = 17,
                Name = "Ventas por vendedor",
                Data = await _service.GetSaleBySellerAsync(userId)
            }
        };

                return Ok(tasks.ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("Movil/GetDataDasboardTest{userId}")]
        public async Task<IActionResult> GetDataDasboardTest(int userId)
        {
            try
            {
                var resutl = await _service.GetIncomePerDayAsync(userId);
                return Ok(resutl);
            }
            catch {
                return BadRequest("Error");
            }
        }
    }
}
