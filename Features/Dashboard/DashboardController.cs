using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pos.WebApi.Features.Dashboard.Dto;
using System;
using System.Collections.Generic;

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
        public IActionResult GetDataDasboard(int userId)
        {
            try
            {
                List<DasboardDataModel> dashboardData = new List<DasboardDataModel>
                {
                    new DasboardDataModel { Id = 1, Name = "Clientes", Data = _service.GetCustomerActive() },
                    new DasboardDataModel { Id = 2, Name = "Proveedores", Data = _service.GetSupplierActive() },
                    new DasboardDataModel { Id = 3, Name = "Articulos", Data = _service.GetItemActive() },
                    new DasboardDataModel { Id = 4, Name = "Almacenes", Data = _service.GetWareHouseActive() },
                    new DasboardDataModel { Id = 5, Name = "Ventas por dia", Data = _service.GetIncomePerDay(userId) },
                    new DasboardDataModel { Id = 6, Name = "Pedidos por vendedor", Data = _service.GetOrderBySeller(userId) },
                    new DasboardDataModel { Id = 7, Name = "Inventario por almacen", Data = _service.GetStockWareHouse() },
                    new DasboardDataModel { Id = 8, Name = "Inventario por categoria", Data = _service.GetStockType() },
                    new DasboardDataModel { Id = 9, Name = "Facturacion por mes", Data = _service.GetInvoicePerMonth(userId) },
                    new DasboardDataModel { Id = 10, Name = "Rentabilidad por mes", Data = _service.GetCostEffectivenessPerMonth(userId) },
                    new DasboardDataModel { Id = 11, Name = "Valor Cuentas por Cobrar", Data = _service.GetCXC(userId) },
                    new DasboardDataModel { Id = 12, Name = "Top 5 articulos mas vendidos", Data = _service.GetItemBestSells(userId) },
                    new DasboardDataModel { Id = 13, Name = "Valor Cuentas por Pagar", Data = _service.GetCXP(userId) },
                    new DasboardDataModel { Id = 14, Name = "Top 5 articulos mas comprados", Data = _service.GetItemBestPurchse(userId) },
                    new DasboardDataModel { Id = 15, Name = "Compras por dia", Data = _service.GetIncomePurchasePerDay(userId) },
                    new DasboardDataModel { Id = 16, Name = "Compras por mes", Data = _service.GetInvoicePurchasePerMonth(userId), },
                    new DasboardDataModel { Id = 17, Name = "Ventas por vendedor", Data = _service.GetSaleBySeller(userId) },
                };
                return Ok(dashboardData);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
