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

namespace Pos.WebApi.Features.Dashboard
{
    public class DashboardServices
    {
        private readonly CustomerServices _customerServices;
        private readonly SupplierServices _supplierServices;
        private readonly ItemServices _itemServices;
        private readonly PurchaseServices _purchaseServices;
        private readonly SalesServices _salesServices;
        private readonly PaymentSaleServices _paymentSaleServices;
        private readonly PaymentPurchaseServices _paymentPurchaseServices;
        private readonly PosDbContext _context;

        public DashboardServices(CustomerServices customerServices, SupplierServices supplierServices, ItemServices itemServices, PurchaseServices purchaseServices, SalesServices salesServices, PaymentSaleServices paymentSaleServices, PaymentPurchaseServices paymentPurchaseServices, PosDbContext posDbContext)
        {
            _customerServices = customerServices;
            _supplierServices = supplierServices;
            _itemServices = itemServices;
            _purchaseServices = purchaseServices;
            _salesServices = salesServices;
            _paymentSaleServices = paymentSaleServices;
            _paymentPurchaseServices = paymentPurchaseServices;
            _context = posDbContext;
        }

        public List<DashboardDto> GetCustomerActive()
        {
            int activeCustomerCount = _context.Customer.Count(x => x.Active);
            var result = new List<DashboardDto>
            {
                new DashboardDto { Description = "Clientes", Qty = activeCustomerCount }
            };
            return result;
        }

        public List<DashboardDto> GetSupplierActive()
        {
            int activeSupplierCount = _context.Supplier.Count(x => x.Active);
            var result = new List<DashboardDto>
            {
                new DashboardDto { Description = "Proveedores", Qty = activeSupplierCount }
            };
            return result;
        }

        public List<DashboardDto> GetItemActive()
        {
            int activeItemCount = _context.Item.Count(x => x.Active);
            var result = new List<DashboardDto>
            {
                new DashboardDto { Description = "Articulos", Qty = activeItemCount }
            };
            return result;
        }

        public List<DashboardDto> GetWareHouseActive()
        {
            int activeWareHouseCount = _context.WareHouse.Count(x => x.Active);
            var result = new List<DashboardDto>
            {
                new DashboardDto { Description = "Almacenes", Qty = activeWareHouseCount }
            };
            return result;
        }

        public List<DashboardDto> GetIncomePerDay(int userId)
        {
            var rol = _context.User.Where(x => x.UserId == userId).FirstOrDefault().RoleId;

            DateTime startDate = DateTime.Today.AddDays(-6);
            var last7Days = Enumerable.Range(0, 7).Select(i => startDate.AddDays(i)).ToList();
            var invoice = _context.InvoiceSale
                .Where(x => x.DocDate >= startDate && (rol == 1 || x.CreateBy == userId) && !x.Canceled)
                .GroupBy(x => x.DocDate.Date)
                .Select(g => new DashboardDto { Description = CultureInfo.GetCultureInfo("es-ES").TextInfo.ToTitleCase(g.Key.ToString("dddd")), Qty = g.Sum(x => x.SubTotal) })
                .ToList();
            var result = last7Days.GroupJoin(
                invoice,
                x => CultureInfo.GetCultureInfo("es-ES").TextInfo.ToTitleCase(x.ToString("dddd")),
                y => y.Description,
                (x, y) => new DashboardDto { Description = CultureInfo.GetCultureInfo("es-ES").TextInfo.ToTitleCase(x.ToString("dddd")), Qty = y.Sum(z => z?.Qty ?? 0) }
            ).ToList();
            return result;
        }

        public List<DashboardDto> GetOrderBySeller(int userId)
        {
            var rol = _context.User.Where(x => x.UserId == userId).FirstOrDefault().RoleId;
            var result = _context
                .OrderSale
                .Join(_context.Seller, os => os.SellerId, s => s.SellerId, (os, s) => new { OrderSale = os, Seller = s })
                .Where(x => x.OrderSale.DocDate.Month == DateTime.Now.Month && (rol == 1 || x.OrderSale.CreateBy == userId) && !x.OrderSale.Canceled)
                .GroupBy(x => x.Seller.SellerName)
                .Select(g => new DashboardDto { Description = g.Key, Qty = g.Count() })
                .ToList();
            return result;
        }

        public List<DashboardDto> GetStockWareHouse()
        {
            var result = _context
                .ItemWareHouse
                .Join(_context.WareHouse, iw => iw.WhsCode, wh => wh.WhsCode, (iw, wh) => new { ItemWareHouse = iw, WareHouse = wh })
                .GroupBy(x => x.WareHouse.WhsName)
                .Select(g => new DashboardDto { Description = g.Key, Qty = g.Sum(x => x.ItemWareHouse.Stock) })
                .ToList();

            return result;
        }

        public List<DashboardDto> GetStockType()
        {
            var result = _context
                .ItemWareHouse
                .Join(_context.Item, iw => iw.ItemId, item => item.ItemId, (iw, item) => new { ItemWareHouse = iw, Item = item })
                .Join(_context.ItemCategory, combined => combined.Item.ItemCategoryId, ic => ic.ItemCategoryId, (combined, ic) => new { ItemWareHouse = combined.ItemWareHouse, ItemCategory = ic })
                .GroupBy(x => x.ItemCategory.ItemCategoryName) // Agrupar por el nombre de la categoría
                .Select(g => new DashboardDto { Description = g.Key.ToString(), Qty = g.Sum(x => x.ItemWareHouse.Stock) })
                .ToList();
            return result;
        }

        public List<DashboardDto> GetInvoicePerMonth(int userId)
        {
            var rol = _context.User.Where(x => x.UserId == userId).FirstOrDefault().RoleId;
            var currentYear = DateTime.Now.Year;

            var invoicesByMonth = (
                from month in Enumerable.Range(1, 12)
                let monthStart = new DateTime(currentYear, month, 1)
                join invoice in _context.InvoiceSale.Where(x => !x.Canceled)
                    on new { Month = monthStart.Month, Year = monthStart.Year }
                    equals new { Month = invoice.DocDate.Month, Year = invoice.DocDate.Year }
                    into invoicesInMonth
                from invoice in invoicesInMonth.DefaultIfEmpty()
                where invoice == null || (invoice.DocDate.Year == currentYear && !invoice.Canceled && (rol == 1 || invoice.CreateBy == userId))
                group invoice by CultureInfo.CurrentCulture.TextInfo.ToTitleCase(monthStart.ToString("MMMM")) into monthGroup
                select new DashboardDto
                {
                    Description = monthGroup.Key,
                    Qty = monthGroup.Sum(v => v?.SubTotal ?? 0) // Handle null invoices
                })
                .OrderBy(v => DateTime.ParseExact(v.Description, "MMMM", CultureInfo.CurrentCulture).Month)
                .ToList();

            return invoicesByMonth;
        }

        public List<DashboardDto> GetCostEffectivenessPerMonth(int userId)
        {
            var rol = _context.User.Where(x => x.UserId == userId).FirstOrDefault().RoleId;
            var currentYear = DateTime.Now.Year;
            var months = new[] { "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre" }
                .Select((m, i) => new DashboardDto { Description = m, Qty = 0 }).ToList();
            var query = (from i in _context.InvoiceSale
                         join id in _context.InvoiceSaleDetail on i.DocId equals id.DocId
                         where !i.Canceled && i.DocDate.Year == currentYear && (rol == 1 || i.CreateBy == userId)
                         group new { id.Cost, id.Quantity, id.LineTotal } by (i.DocDate.Month) into g
                         select new DashboardDto
                         {
                             Description = g.Key.ToString(),
                             Qty = ((g.Sum(x => x.LineTotal) - g.Sum(x => x.Cost * x.Quantity)) / g.Sum(x => x.LineTotal)) * 100
                         });

            //Execute LINQ query
            var result = query.ToList();
            var finalResult = months.GroupJoin(result,
                m => (Array.IndexOf(months.Select(x => x.Description).ToArray(), m.Description) + 1).ToString(),
                r => r.Description,
                (m, r) => new DashboardDto { Description = m.Description, Qty = r.FirstOrDefault()?.Qty ?? 0 }).ToList();
            return finalResult;
        }

        public List<DashboardDto> GetSaleBySeller(int userId)
        {
            var rol = _context.User.Where(x => x.UserId == userId).FirstOrDefault().RoleId;
            var result = _context
                .InvoiceSale
                .Join(_context.Seller, os => os.SellerId, s => s.SellerId, (os, s) => new { OrderSale = os, Seller = s })
                .Where(x => x.OrderSale.DocDate.Month == DateTime.Now.Month && !x.OrderSale.Canceled && (rol == 1 || x.OrderSale.CreateBy == userId))
                .GroupBy(x => x.Seller.SellerName)
                .Select(g => new DashboardDto { Description = g.Key, Qty = g.Sum(x=> x.OrderSale.SubTotal)})
                .ToList();
            return result;
        }
        public List<DashboardDto> GetCXC(int userId)
        {
            var rol = _context.User
               .Where(x => x.UserId == userId)
               .Select(x => new { x.RoleId, x.SellerId})
               .FirstOrDefault();

            decimal balanceCustomers = _context.InvoiceSale
                .Where(x=> (rol.RoleId == 1 || x.SellerId == rol.SellerId))
                .Sum(x => x.Balance);

            var result = new List<DashboardDto>
            {
                new DashboardDto { Description = "CXC", Qty = balanceCustomers }
            };
            return result;
        }
        public List<DashboardDto> GetItemBestSells(int userId)
        {
            var rol = _context.User
                .Where(x => x.UserId == userId)
                .Select(x => x.RoleId)
                .FirstOrDefault();

            var result = (from i in _context.InvoiceSale
                          join id in _context.InvoiceSaleDetail on i.DocId equals id.DocId
                          join it in _context.Item on id.ItemId equals it.ItemId
                          where (rol == 1 || i.CreateBy == userId) && i.DocDate.Year == DateTime.Now.Year && !i.Canceled
                          group new { id.LineTotal, it.ItemName, it.ItemCode } by id.ItemId into grouped
                          orderby grouped.Sum(x => x.LineTotal) descending
                          select new DashboardDto
                          {
                              Description = $"{grouped.First().ItemCode} - {grouped.First().ItemName}",
                              Qty = grouped.Sum(x => x.LineTotal)
                          })
                          .Take(5)
                          .ToList();
            return result;
        }

        //Purchase
        public List<DashboardDto> GetIncomePurchasePerDay(int userId)
        {
            var rol = _context.User.Where(x => x.UserId == userId).FirstOrDefault().RoleId;
            DateTime startDate = DateTime.Today.AddDays(-6);
            var last7Days = Enumerable.Range(0, 7).Select(i => startDate.AddDays(i)).ToList();
            var invoice = _context.InvoicePurchase
                .Where(x => x.DocDate >= startDate && (rol == 1 || x.CreateBy == userId) && !x.Canceled)
                .GroupBy(x => x.DocDate.Date)
                .Select(g => new DashboardDto { Description = CultureInfo.GetCultureInfo("es-ES").TextInfo.ToTitleCase(g.Key.ToString("dddd")), Qty = g.Sum(x => x.SubTotal) })
                .ToList();
            var result = last7Days.GroupJoin(
                invoice,
                x => CultureInfo.GetCultureInfo("es-ES").TextInfo.ToTitleCase(x.ToString("dddd")),
                y => y.Description,
                (x, y) => new DashboardDto { Description = CultureInfo.GetCultureInfo("es-ES").TextInfo.ToTitleCase(x.ToString("dddd")), Qty = y.Sum(z => z?.Qty ?? 0) }
            ).ToList();
            return result;
        }
        public List<DashboardDto> GetInvoicePurchasePerMonth(int userId)
        {
            var rol = _context.User.Where(x => x.UserId == userId).FirstOrDefault().RoleId;
            var currentYear = DateTime.Now.Year;

            var invoicesByMonth = (
                from month in Enumerable.Range(1, 12)
                let monthStart = new DateTime(currentYear, month, 1)
                join invoice in _context.InvoicePurchase.Where(x => !x.Canceled)
                    on new { Month = monthStart.Month, Year = monthStart.Year }
                    equals new { Month = invoice.DocDate.Month, Year = invoice.DocDate.Year }
                    into invoicesInMonth
                from invoice in invoicesInMonth.DefaultIfEmpty()
                where invoice == null || (invoice.DocDate.Year == currentYear && !invoice.Canceled && (rol == 1 || invoice.CreateBy == userId))
                group invoice by CultureInfo.CurrentCulture.TextInfo.ToTitleCase(monthStart.ToString("MMMM")) into monthGroup
                select new DashboardDto
                {
                    Description = monthGroup.Key,
                    Qty = monthGroup.Sum(v => v?.SubTotal ?? 0) // Handle null invoices
                })
                .OrderBy(v => DateTime.ParseExact(v.Description, "MMMM", CultureInfo.CurrentCulture).Month)
                .ToList();

            return invoicesByMonth;
        }
        public List<DashboardDto> GetCXP(int userId)
        {
            var rol = _context.User
               .Where(x => x.UserId == userId)
               .Select(x => new { x.RoleId, x.SellerId })
               .FirstOrDefault();
            decimal balanceCustomers = _context.InvoicePurchase
                 .Where(x => (rol.RoleId == 1 || x.CreateBy == userId))
                .Sum(x=> x.Balance);
            var result = new List<DashboardDto>
            {
                new DashboardDto { Description = "CXP", Qty = balanceCustomers }
            };
            return result;
        }
        public List<DashboardDto> GetItemBestPurchse(int userId)
        {
            var rol = _context.User
                .Where(x => x.UserId == userId)
                .Select(x => x.RoleId)
                .FirstOrDefault();

            var result = (from i in _context.InvoicePurchase
                          join id in _context.InvoicePurchaseDetail on i.DocId equals id.DocId
                          join it in _context.Item on id.ItemId equals it.ItemId
                          where (rol == 1 || i.CreateBy == userId) && i.DocDate.Year == DateTime.Now.Year 
                          group new { id.LineTotal, it.ItemName, it.ItemCode } by id.ItemId into grouped
                          orderby grouped.Sum(x => x.LineTotal) descending
                          select new DashboardDto
                          {
                              Description = $"{grouped.First().ItemCode} - {grouped.First().ItemName}",
                              Qty = grouped.Sum(x => x.LineTotal)
                          })
                          .Take(5)
                          .ToList();
            return result;
        }



















    }
}
