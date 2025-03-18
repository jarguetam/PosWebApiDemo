using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Pos.WebApi.Features.Customers.Services;
using Pos.WebApi.Features.Dashboard.Dto;
using Pos.WebApi.Features.Items.Services;
using Pos.WebApi.Features.Purchase.Services;
using Pos.WebApi.Features.PurchasePayment.Service;
using Pos.WebApi.Features.Sales.Services;
using Pos.WebApi.Features.SalesPayment.Service;
using Pos.WebApi.Features.Suppliers.Services;
using Pos.WebApi.Infraestructure;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Pos.WebApi.Features.Dashboard
{
    public class DashboardServices
    {
        private readonly PosDbContext _context;
        private readonly IMemoryCache _cache;

        public DashboardServices(PosDbContext posDbContext, IMemoryCache cache)
        {
            _context = posDbContext;
            _cache = cache;
        }

        public async Task<List<DashboardDto>> GetCustomerActiveAsync()
        {
            string cacheKey = "CustomerActive";
            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromMinutes(30);
                var count = await _context.Customer.CountAsync(x => x.Active);
                return new List<DashboardDto>
            {
                new DashboardDto { Description = "Clientes", Qty = count }
            };
            });
        }

        public async Task<List<DashboardDto>> GetSupplierActiveAsync()
        {
            string cacheKey = "SupplierActive";
            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromMinutes(30);
                var count = await _context.Supplier.CountAsync(x => x.Active);
                return new List<DashboardDto>
            {
                new DashboardDto { Description = "Proveedores", Qty = count }
            };
            });
        }

        public async Task<List<DashboardDto>> GetItemActiveAsync()
        {
            string cacheKey = "ItemActive";
            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromMinutes(30);
                var count = await _context.Item.CountAsync(x => x.Active);
                return new List<DashboardDto>
            {
                new DashboardDto { Description = "Articulos", Qty = count }
            };
            });
        }

        public async Task<List<DashboardDto>> GetWareHouseActiveAsync()
        {
            string cacheKey = "WareHouseActive";
            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromMinutes(30);
                var count = await _context.WareHouse.CountAsync(x => x.Active);
                return new List<DashboardDto>
            {
                new DashboardDto { Description = "Almacenes", Qty = count }
            };
            });
        }

        public async Task<List<DashboardDto>> GetIncomePerDayAsync(int userId)
        {
            var rol = await _context.User
                .Where(x => x.UserId == userId)
                .Select(x => new { x.RoleId,x.SellerId })
                .FirstOrDefaultAsync();

            DateTime startDate = DateTime.Today.AddDays(-6);
            var last7Days = Enumerable.Range(0, 7).Select(i => startDate.AddDays(i)).ToList();

            var invoice = await _context.InvoiceSale
                .Where(x => x.DocDate >= startDate && (rol.RoleId == 1 || x.SellerId == rol.SellerId) && !x.Canceled)
                .GroupBy(x => x.DocDate.Date)
                .Select(g => new DashboardDto
                {
                    Description = CultureInfo.GetCultureInfo("es-ES").TextInfo.ToTitleCase(g.Key.ToString("dddd")),
                    Qty = g.Sum(x => x.SubTotal)
                })
                .ToListAsync();

            return last7Days
                .GroupJoin(
                    invoice,
                    x => CultureInfo.GetCultureInfo("es-ES").TextInfo.ToTitleCase(x.ToString("dddd")),
                    y => y.Description,
                    (x, y) => new DashboardDto
                    {
                        Description = CultureInfo.GetCultureInfo("es-ES").TextInfo.ToTitleCase(x.ToString("dddd")),
                        Qty = y.Sum(z => z?.Qty ?? 0)
                    }
                ).ToList();
        }

        public async Task<List<DashboardDto>> GetStockWareHouseAsync()
        {
            string cacheKey = $"StockWareHouse_{DateTime.Now.Date}";
            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromHours(1);
                return await _context.ItemWareHouse
                    .Join(_context.WareHouse,
                        iw => iw.WhsCode,
                        wh => wh.WhsCode,
                        (iw, wh) => new { ItemWareHouse = iw, WareHouse = wh })
                    .GroupBy(x => x.WareHouse.WhsName)
                    .Select(g => new DashboardDto
                    {
                        Description = g.Key,
                        Qty = g.Sum(x => x.ItemWareHouse.Stock)
                    })
                    .ToListAsync();
            });
        }
        public async Task<List<DashboardDto>> GetStockTypeAsync()
        {
            string cacheKey = $"StockType_{DateTime.Now.Date}";
            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromHours(1);
                return await _context.ItemWareHouse
                    .Join(_context.Item,
                        iw => iw.ItemId,
                        item => item.ItemId,
                        (iw, item) => new { ItemWareHouse = iw, Item = item })
                    .Join(_context.ItemCategory,
                        combined => combined.Item.ItemCategoryId,
                        ic => ic.ItemCategoryId,
                        (combined, ic) => new { ItemWareHouse = combined.ItemWareHouse, ItemCategory = ic })
                    .GroupBy(x => x.ItemCategory.ItemCategoryName)
                    .Select(g => new DashboardDto
                    {
                        Description = g.Key,
                        Qty = g.Sum(x => x.ItemWareHouse.Stock)
                    })
                    .ToListAsync();
            });
        }

        public async Task<List<DashboardDto>> GetOrderBySellerAsync(int userId)
        {
            var rol = await _context.User
                .Where(x => x.UserId == userId)
                .Select(x => x.RoleId)
                .FirstOrDefaultAsync();

            return await _context.OrderSale
                .Join(_context.Seller,
                    os => os.SellerId,
                    s => s.SellerId,
                    (os, s) => new { OrderSale = os, Seller = s })
                .Where(x => x.OrderSale.DocDate.Month == DateTime.Now.Month &&
                           (rol == 1 || x.OrderSale.CreateBy == userId) &&
                           !x.OrderSale.Canceled)
                .GroupBy(x => x.Seller.SellerName)
                .Select(g => new DashboardDto
                {
                    Description = g.Key,
                    Qty = g.Count()
                })
                .ToListAsync();
        }

        public async Task<List<DashboardDto>> GetInvoicePerMonthAsync(int userId)
        {
            var rol = await _context.User
                .Where(x => x.UserId == userId)
                .Select(x => new { x.RoleId, x.SellerId})
                .FirstOrDefaultAsync();

            var currentYear = DateTime.Now.Year;
            var months = Enumerable.Range(1, 12)
                .Select(month => new DateTime(currentYear, month, 1))
                .ToList();

            var invoices = await _context.InvoiceSale
                .Where(x => x.DocDate.Year == currentYear &&
                           !x.Canceled &&
                           (rol.RoleId == 1 || x.SellerId == rol.SellerId))
                .GroupBy(x => x.DocDate.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    Total = g.Sum(x => x.SubTotal)
                })
                .ToListAsync();

            return months
                .Select(monthDate => new DashboardDto
                {
                    Description = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(monthDate.ToString("MMMM")),
                    Qty = invoices.FirstOrDefault(i => i.Month == monthDate.Month)?.Total ?? 0
                })
                .ToList();
        }

        public async Task<List<DashboardDto>> GetCostEffectivenessPerMonthAsync(int userId)
        {
            var rol = await _context.User
                .Where(x => x.UserId == userId)
                .Select(x => x.RoleId)
                .FirstOrDefaultAsync();

            var currentYear = DateTime.Now.Year;
            var months = new[] { "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
                            "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre" };

            var effectivenessData = await _context.InvoiceSale
                .Where(x => !x.Canceled &&
                           x.DocDate.Year == currentYear &&
                           (rol == 1 || x.CreateBy == userId))
                .Join(_context.InvoiceSaleDetail,
                    i => i.DocId,
                    id => id.DocId,
                    (i, id) => new { i.DocDate, id.Cost, id.Quantity, id.LineTotal })
                .GroupBy(x => x.DocDate.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    Effectiveness = ((g.Sum(x => x.LineTotal) - g.Sum(x => x.Cost * x.Quantity)) /
                                    g.Sum(x => x.LineTotal)) * 100
                })
                .ToListAsync();

            return months
                .Select((month, index) => new DashboardDto
                {
                    Description = month,
                    Qty = effectivenessData.FirstOrDefault(x => x.Month == index + 1)?.Effectiveness ?? 0
                })
                .ToList();
        }

        public async Task<List<DashboardDto>> GetCXCAsync(int userId)
        {
            var userInfo = await _context.User
                .Where(x => x.UserId == userId)
                .Select(x => new { x.RoleId, x.SellerId })
                .FirstOrDefaultAsync();

            var balanceCustomers = await _context.InvoiceSale
                .Where(x => (userInfo.RoleId == 1 || x.SellerId == userInfo.SellerId))
                .SumAsync(x => x.Balance);

            return new List<DashboardDto>
        {
            new DashboardDto { Description = "CXC", Qty = balanceCustomers }
        };
        }

        public async Task<List<DashboardDto>> GetItemBestSellsAsync(int userId)
        {
            var rol = await _context.User
                .Where(x => x.UserId == userId)
                .Select(x => x.RoleId)
                .FirstOrDefaultAsync();

            return await _context.InvoiceSale
                .Where(i => (rol == 1 || i.CreateBy == userId) &&
                           i.DocDate.Year == DateTime.Now.Year &&
                           !i.Canceled)
                .Join(_context.InvoiceSaleDetail,
                    i => i.DocId,
                    id => id.DocId,
                    (i, id) => id)
                .Join(_context.Item,
                    id => id.ItemId,
                    it => it.ItemId,
                    (id, it) => new { id.LineTotal, it.ItemName, it.ItemCode, it.ItemId })
                .GroupBy(x => x.ItemId)
                .Select(g => new DashboardDto
                {
                    Description = $"{g.First().ItemCode} - {g.First().ItemName}",
                    Qty = g.Sum(x => x.LineTotal)
                })
                .OrderByDescending(x => x.Qty)
                .Take(5)
                .ToListAsync();
        }

        // Métodos para compras
        public async Task<List<DashboardDto>> GetIncomePurchasePerDayAsync(int userId)
        {
            var rol = await _context.User
                .Where(x => x.UserId == userId)
                .Select(x => x.RoleId)
                .FirstOrDefaultAsync();

            DateTime startDate = DateTime.Today.AddDays(-6);
            var last7Days = Enumerable.Range(0, 7)
                .Select(i => startDate.AddDays(i))
                .ToList();

            var purchases = await _context.InvoicePurchase
                .Where(x => x.DocDate >= startDate &&
                           (rol == 1 || x.CreateBy == userId) &&
                           !x.Canceled)
                .GroupBy(x => x.DocDate.Date)
                .Select(g => new DashboardDto
                {
                    Description = CultureInfo.GetCultureInfo("es-ES").TextInfo.ToTitleCase(g.Key.ToString("dddd")),
                    Qty = g.Sum(x => x.SubTotal)
                })
                .ToListAsync();

            return last7Days
                .Select(date => new DashboardDto
                {
                    Description = CultureInfo.GetCultureInfo("es-ES").TextInfo.ToTitleCase(date.ToString("dddd")),
                    Qty = purchases.FirstOrDefault(p =>
                        p.Description == CultureInfo.GetCultureInfo("es-ES").TextInfo.ToTitleCase(date.ToString("dddd")))?.Qty ?? 0
                })
                .ToList();
        }
        public async Task<List<DashboardDto>> GetInvoicePurchasePerMonthAsync(int userId)
        {
            var rol = await _context.User
                .Where(x => x.UserId == userId)
                .Select(x => x.RoleId)
                .FirstOrDefaultAsync();

            var currentYear = DateTime.Now.Year;
            var months = Enumerable.Range(1, 12)
                .Select(month => new DateTime(currentYear, month, 1))
                .ToList();

            var purchases = await _context.InvoicePurchase
                .Where(x => x.DocDate.Year == currentYear &&
                           !x.Canceled &&
                           (rol == 1 || x.CreateBy == userId))
                .GroupBy(x => x.DocDate.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    Total = g.Sum(x => x.SubTotal)
                })
                .ToListAsync();

            return months
                .Select(monthDate => new DashboardDto
                {
                    Description = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(monthDate.ToString("MMMM")),
                    Qty = purchases.FirstOrDefault(i => i.Month == monthDate.Month)?.Total ?? 0
                })
                .OrderBy(x => DateTime.ParseExact(x.Description, "MMMM", CultureInfo.CurrentCulture).Month)
                .ToList();
        }

        public async Task<List<DashboardDto>> GetCXPAsync(int userId)
        {
            var userInfo = await _context.User
                .Where(x => x.UserId == userId)
                .Select(x => new { x.RoleId, x.SellerId })
                .FirstOrDefaultAsync();

            var balanceSuppliers = await _context.InvoicePurchase
                .Where(x => (userInfo.RoleId == 1 || x.CreateBy == userId))
                .SumAsync(x => x.Balance);

            return new List<DashboardDto>
        {
            new DashboardDto { Description = "CXP", Qty = balanceSuppliers }
        };
        }

        public async Task<List<DashboardDto>> GetItemBestPurchaseAsync(int userId)
        {
            var rol = await _context.User
                .Where(x => x.UserId == userId)
                .Select(x => x.RoleId)
                .FirstOrDefaultAsync();

            return await _context.InvoicePurchase
                .Where(i => (rol == 1 || i.CreateBy == userId) &&
                           i.DocDate.Year == DateTime.Now.Year)
                .Join(_context.InvoicePurchaseDetail,
                    i => i.DocId,
                    id => id.DocId,
                    (i, id) => id)
                .Join(_context.Item,
                    id => id.ItemId,
                    it => it.ItemId,
                    (id, it) => new { id.LineTotal, it.ItemName, it.ItemCode, it.ItemId })
                .GroupBy(x => x.ItemId)
                .Select(g => new DashboardDto
                {
                    Description = $"{g.First().ItemCode} - {g.First().ItemName}",
                    Qty = g.Sum(x => x.LineTotal)
                })
                .OrderByDescending(x => x.Qty)
                .Take(5)
                .ToListAsync();
        }

        public async Task<List<DashboardDto>> GetSaleBySellerAsync(int userId)
        {
            var rol = await _context.User
                .Where(x => x.UserId == userId)
                .Select(x => new { x.RoleId , x.SellerId})
                .FirstOrDefaultAsync();

            return await _context.InvoiceSale
                .Where(x => x.DocDate.Month == DateTime.Now.Month &&
                           !x.Canceled &&
                           (rol.RoleId == 1 || x.SellerId == rol.SellerId))
                .Join(_context.Seller,
                    os => os.SellerId,
                    s => s.SellerId,
                    (os, s) => new { OrderSale = os, Seller = s })
                .GroupBy(x => x.Seller.SellerName)
                .Select(g => new DashboardDto
                {
                    Description = g.Key,
                    Qty = g.Sum(x => x.OrderSale.SubTotal)
                })
                .ToListAsync();
        }
    }
}
