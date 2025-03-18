using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Pos.WebApi.Features.Reports.Dto;
using Pos.WebApi.Helpers;
using Pos.WebApi.Infraestructure;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;


namespace Pos.WebApi.Features.Reports.Services
{
    public class ReportServices
    {
        private readonly PosDbContext _context;
        private readonly IConfiguration _config;
        public ReportServices(PosDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public List<InventoryReportDto> GetInventoryReports()
        {
            var result = (from iw in _context.ItemWareHouse
                          join i in _context.Item on iw.ItemId equals i.ItemId
                          group iw by new { i.ItemCode,i.ItemName, i.AvgPrice } into g
                          orderby g.Key.ItemCode
                          select new InventoryReportDto
                          {
                              ItemCode = g.Key.ItemCode,
                              ItemName = g.Key.ItemName,
                              Quantity = g.Sum(iw => iw.Stock),
                              AvgPrice = g.Key.AvgPrice,
                              CostTotal = g.Sum(iw => iw.AvgPrice * iw.Stock)
                          }).ToList();
            return result;
        }

        public List<InventoryReportDto> GetInventoryWarehouseReports()
        {
            var result = (from iw in _context.ItemWareHouse
                          join i in _context.Item on iw.ItemId equals i.ItemId
                          join w in _context.WareHouse on iw.WhsCode equals w.WhsCode
                          group iw by new { i.ItemCode,i.ItemName, iw.AvgPrice, w.WhsName } into g
                          orderby g.Key.WhsName, g.Key.ItemCode
                          select new InventoryReportDto
                          {
                              ItemCode = g.Key.ItemCode,
                              ItemName = g.Key.ItemName,
                              Quantity = g.Sum(iw => iw.Stock),
                              WhsName = g.Key.WhsName,
                              AvgPrice = g.Key.AvgPrice,
                              CostTotal = g.Sum(iw => iw.AvgPrice * iw.Stock)
                          }).ToList();
            return result;
        }

        public void GetInventoryWarehouseReportsPdf()
        {
            var result = (from iw in _context.ItemWareHouse
                          join i in _context.Item on iw.ItemId equals i.ItemId
                          join w in _context.WareHouse on iw.WhsCode equals w.WhsCode
                          group iw by new { i.ItemCode, i.ItemName, i.AvgPrice, w.WhsName } into g
                          orderby g.Key.WhsName, g.Key.ItemCode
                          select new InventoryReportDto
                          {
                              ItemCode = g.Key.ItemCode,
                              ItemName = g.Key.ItemName,
                              Quantity = g.Sum(iw => iw.Stock),
                              WhsName = g.Key.WhsName,
                              AvgPrice = g.Key.AvgPrice,
                              CostTotal = g.Sum(iw => iw.AvgPrice * iw.Stock)
                          }).ToList();
        }

        public MemoryStream GenerateReportSalesDate(DateTime fro, DateTime to)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            var companyInfo = _context.CompanyInfo.FirstOrDefault();
            var file = _context.FileUpload.Where(x => x.FileId == companyInfo.FileId).FirstOrDefault();
            var result = (from f in _context.InvoiceSale
                          join fd in _context.InvoiceSaleDetail on f.DocId equals fd.DocId
                          join i in _context.Item on fd.ItemId equals i.ItemId
                          join w in _context.WareHouse on fd.WhsCode equals w.WhsCode
                          join s in _context.Seller on f.SellerId equals s.SellerId
                          join pc in _context.PayCondition on f.PayConditionId equals pc.PayConditionId
                          join zs in _context.SellerRegion on s.RegionId equals zs.RegionId
                          where f.DocDate.Date >= fro.Date && f.DocDate.Date <= to.Date
                          && f.Canceled ==false
                          orderby f.DocDate
                          select new SalesReportDto
                          {
                              InvoiceFiscalNo = f.InvoiceFiscalNo,
                              DocDate = f.DocDate,
                              CustomerCode = f.CustomerCode,
                              CustomerName = f.CustomerName,
                              PayConditionName = pc.PayConditionName,
                              ItemCode = i.ItemCode,
                              ItemName = i.ItemName,
                              Quantity = fd.Quantity,
                              Price = fd.Price,
                              LineTotal = fd.LineTotal,
                              WhsName = w.WhsName,
                              SellerName = s.SellerName,
                              SellerZone = zs.NameRegion
                          }).ToList();
            var report = new SalesReportService(result, fro, to, companyInfo, file);
            byte[] pdfBytes = report.ComposeReportSale();
            MemoryStream ms = new(pdfBytes);
            return ms;
        }

        public MemoryStream GenerateReportCxc(int sellerId, bool onlyOverdue = false)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            string sellerName = sellerId == 0
                ? "Todos los vendedores"
                : _context.Seller.FirstOrDefault(x => x.SellerId == sellerId)?.SellerName;
            var companyInfo = _context.CompanyInfo.FirstOrDefault();
            var file = _context.FileUpload.Where(x => x.FileId == companyInfo.FileId).FirstOrDefault();
            var today = DateTime.Today;

            // Ejecutar la consulta para obtener los resultados de la base de datos
            var queryResults = (from invoice in _context.InvoiceSale
                                join seller in _context.Seller on invoice.SellerId equals seller.SellerId
                                join customer in _context.Customer on invoice.CustomerId equals customer.CustomerId
                                where invoice.Balance > 0
                                      && !invoice.Canceled
                                      && (sellerId == 0 || invoice.SellerId == sellerId)
                                      && (!onlyOverdue || invoice.DueDate.Date <= today)
                                group new { invoice, seller } by new { invoice.CustomerCode, invoice.CustomerName, customer.Address } into g
                                orderby g.Key.CustomerName
                                select new
                                {
                                    CustomerCode = g.Key.CustomerCode,
                                    CustomerName = $"{g.Key.CustomerName} - {g.Key.Address}",
                                    Details = g.Select(x => new
                                    {
                                        DocNum = x.invoice.DocId,
                                        Reference = x.invoice.Reference,
                                        DocDate = x.invoice.DocDate,
                                        DocDueDate = x.invoice.DueDate,
                                        DocTotal = x.invoice.DocTotal,
                                        Balance = x.invoice.Balance,
                                        SellerName = x.seller.SellerName
                                    }).ToList()
                                }).ToList();

            // Calcular los comentarios fuera de la consulta LINQ
            var result = queryResults.Select(group =>
            {
                return new CxcreportDto
                {
                    CustomerCode = group.CustomerCode,
                    CustomerName = group.CustomerName,
                    Detail = group.Details.Select(detail =>
                    {
                        var dueDate = detail.DocDueDate;
                        return new CxcreportDetailDto
                        {
                            DocNum = detail.DocNum,
                            Reference = detail.Reference,
                            DocDate = detail.DocDate,
                            DocDueDate = dueDate,
                            DocTotal = detail.DocTotal,
                            Balance = detail.Balance,
                            Comment = GetCommentForDueDate(dueDate),
                            SellerName = detail.SellerName,
                            Days = GetDaysForDueDate(dueDate)
                        };
                    }).ToList()
                };
            }).ToList();

            // Métodos auxiliares
            string GetCommentForDueDate(DateTime dueDate)
            {
                TimeSpan difference = dueDate.Date - DateTime.Today;
                if (difference.Days == 0)
                {
                    return "**Vence hoy**";
                }
                else if (difference.Days > 0)
                {
                    return $"Vence en {difference.Days} días";
                }
                else
                {
                    return $"Venció hace {-difference.Days} días";
                }
            }

            int GetDaysForDueDate(DateTime dueDate)
            {
                TimeSpan difference = dueDate.Date - DateTime.Today;
                return difference.Days;
            }

            var report = new CxcReportServices(result, companyInfo, file, sellerName);
            byte[] pdfBytes = report.ComposeReportSale();
            MemoryStream ms = new(pdfBytes);
            return ms;
        }

        public MemoryStream GenerateReportInventoryPdf(int whsCode)
        {
            var companyInfo = _context.CompanyInfo.FirstOrDefault();
            var file = _context.FileUpload.Where(x => x.FileId == companyInfo.FileId).FirstOrDefault();

            var sql = @"
            SELECT 
                [ItemCategoryName],
                [ItemCode],
                [ItemName],
                [ItemFamilyName],
                SUM([Stock]) AS Stock,
                AVG([AvgPrice]) AS AvgPrice,
                SUM([Total]) AS Total
            FROM 
                [dbposweb].[dbo].[V_STOCK_WAREHOUSE]
            WHERE 
                [Stock] != 0
                {0}
            GROUP BY 
                [ItemCategoryName],
                [ItemCode],
                [ItemName],
                [ItemFamilyName]
            ORDER BY 
                [ItemCategoryName]";

            string whereClause = whsCode != 0 ? $"AND [WhsCode] = {whsCode}" : "";
            sql = string.Format(sql, whereClause);

            string conec = _config["connectionStrings:dbpos"];
            List<ReportInventoryCategoryDto> lst = new List<ReportInventoryCategoryDto>();
            int totalLines = 0;

            using (var connection = new SqlConnection(conec))
            {
                var inventoryItems = connection.Query<InventoryCategoryDetail>(sql).ToList();
                totalLines = inventoryItems.Count;
                lst = inventoryItems
                    .GroupBy(item => item.ItemFamilyName)
                    .Select(group => new ReportInventoryCategoryDto
                    {
                        ItemCategoryName = group.Key,
                        Detail = group.ToList()
                    })
                    .ToList();
            }

            string whsName = whsCode != 0
                ? _context.WareHouse.Where(x => x.WhsCode == whsCode).FirstOrDefault()?.WhsName
                : "Todos los almacenes";

            var report = new ReportInventoryCategoryServices(lst, companyInfo, file, totalLines, whsName);
            byte[] pdfBytes = report.ComposeReportSale();
            MemoryStream ms = new(pdfBytes);
            return ms;
        }
        public MemoryStream GenerateReporteSalesMargen(DateTime from, DateTime to, int sellerId)
        {
            var companyInfo = _context.CompanyInfo.FirstOrDefault();
            var file = _context.FileUpload.FirstOrDefault(x => x.FileId == companyInfo.FileId);

            var resultados = ObtenerResultadosVenta(from, to, sellerId);

            var reporteVentas = new ReportSalesMargenDto
            {
                Familias = resultados.GroupBy(r => r.Familia)
                    .Select(g => new FamiliaViewModel
                    {
                        Familia = g.Key,
                        SubFamilias = g.GroupBy(s => s.Sub_Familia)
                            .Select(sg => new SubFamiliaViewModel
                            {
                                SubFamilia = sg.Key,
                                Detalles = sg.Select(d => new DetalleVentaViewModel
                                {
                                    Codigo = d.Codigo,
                                    Descripcion = d.Descripcion,
                                    Costo = d.Costo,
                                    Venta = d.Venta,
                                    Cantidad = d.Cantidad,
                                    Libras = d.Libas,
                                    CostoTotal = d.CostoTotal,
                                    VentaTotal = d.VentaTotal,
                                    RentabilidadBruta = d.RentabilidadBruta,
                                    PorcentajeRentabilidad = d.Rent,
                                    Ruta = d.Ruta
                                }).ToList()
                            }).ToList()
                    }).ToList()
            };
            string sellerName = sellerId == 0
            ? "Todos los vendedores"
            : _context.Seller.FirstOrDefault(x => x.SellerId == sellerId)?.SellerName;
            var report = new ReportSalesMargenServices(reporteVentas, companyInfo, file, sellerName, from, to);
            byte[] pdfBytes = report.ComposeReportSale();
            return new MemoryStream(pdfBytes);
        }

        private List<ResultadoVenta> ObtenerResultadosVenta(DateTime from, DateTime to, int sellerId)
        {
            var resultados = new List<ResultadoVenta>();
            string connectionString = _config.GetConnectionString("dbpos");

            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = new SqlCommand("sp_ReporteVentas", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@FechaInicio", SqlDbType.Date).Value = from.Date;
                    command.Parameters.Add("@FechaFin", SqlDbType.Date).Value = to.Date;
                    command.Parameters.Add("@Seller", SqlDbType.Int).Value = sellerId;

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            resultados.Add(new ResultadoVenta
                            {
                                Familia = reader["Familia"].ToString(),
                                Sub_Familia = reader["Sub_Familia"].ToString(),
                                Codigo = reader["Codigo"].ToString(),
                                Descripcion = reader["Descripcion"].ToString(),
                                Costo = Convert.ToDecimal(reader["Costo"]),
                                Venta = Convert.ToDecimal(reader["Venta"]),
                                Cantidad = Convert.ToDecimal(reader["Cantidad"]),
                                Libas = Convert.ToDecimal(reader["Libas"]),
                                CostoTotal = Convert.ToDecimal(reader["CostoTotal"]),
                                VentaTotal = Convert.ToDecimal(reader["VentaTotal"]),
                                RentabilidadBruta = Convert.ToDecimal(reader["RentabilidadBruta"]),
                                Rent = Convert.ToDecimal(reader["Rent%"]),
                            });
                        }
                    }
                }
            }

            return resultados;
        }

        public MemoryStream GenerateReportExpense(DateTime from, DateTime to, int sellerId)
        {
            var companyInfo = _context.CompanyInfo.FirstOrDefault();
            if (companyInfo == null)
            {
                throw new InvalidOperationException("No se encontró información de la compañía.");
            }

            var file = _context.FileUpload.FirstOrDefault(x => x.FileId == companyInfo.FileId);

            string sellerName = sellerId == 0
                ? "Todos los vendedores"
                : _context.Seller.FirstOrDefault(x => x.SellerId == sellerId)?.SellerName ?? "Vendedor desconocido";

            var sql = @"
            SELECT 
                T0.ExpenseTypeId [ExpenseId],
                T0.DESCRIPCION [ExpenseName],
                CASE WHEN T0.ExpenseTypeId = 23 THEN 'ACTIVOS' ELSE 'GASTOS' END AS [ExpenseType],
                SUM(T0.TOTAL) AS [ExpenseAmount]
            FROM [dbposweb].[dbo].[V_GASTOS] T0
            WHERE T0.FECHA BETWEEN @FromDate AND @ToDate 
            AND (@SellerId = 0 OR T0.RUTA = @SellerName)
            AND [ANULADO] ='NO' AND [LINEA ANULADA]='NO'
            GROUP BY T0.ExpenseTypeId, T0.DESCRIPCION
            ORDER BY T0.ExpenseTypeId";

            string connectionString = _config.GetConnectionString("dbpos");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("La cadena de conexión 'dbpos' no está configurada.");
            }

            List<ReportExpenseDto> lst;
            using (var connection = new SqlConnection(connectionString))
            {
                lst = connection.Query<ReportExpenseDto>(sql, new
                {
                    FromDate = from.Date,
                    ToDate = to.Date,
                    SellerId = sellerId,
                    SellerName = sellerName
                }).ToList();
            }

            int totalLines = lst.Count;

            var report = new ReportExpenseService(lst, companyInfo, file, totalLines, sellerName, from, to);
            byte[] pdfBytes = report.ComposeReportSale();
            return new MemoryStream(pdfBytes);
        }

    }
}
