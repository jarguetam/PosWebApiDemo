using Microsoft.EntityFrameworkCore;
using Pos.WebApi.Features.Reports.Dto;
using Pos.WebApi.Features.Sales.Dto;
using Pos.WebApi.Infraestructure;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;

namespace Pos.WebApi.Features.Reports.Services
{
    public class ReportServices
    {
        private readonly PosDbContext _context;

        public ReportServices(PosDbContext context)
        {
            _context = context;
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
                          group iw by new { i.ItemCode,i.ItemName, i.AvgPrice, w.WhsName } into g
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
    }
}
