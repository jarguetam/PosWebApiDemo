using Pos.WebApi.Features.Common.Entities;
using Pos.WebApi.Features.Common;
using Pos.WebApi.Features.Customers.Services;
using Pos.WebApi.Features.InventoryTransactions.Services;
using Pos.WebApi.Features.Items.Services;
using Pos.WebApi.Features.Sales.Dto;
using Pos.WebApi.Features.Sales.Entities;
using Pos.WebApi.Infraestructure;
using Pos.WebApi.Features.InventoryTransactions.Entities;
using Pos.WebApi.Features.Items.Entities;
using Pos.WebApi.Features.SalesPayment.Entities;
using System.Collections.Generic;
using System;
using System.Linq;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;
using Dapper;
using Org.BouncyCastle.Asn1.Ocsp;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace Pos.WebApi.Features.Sales.Services
{
    public class SalesServices
    {
        private readonly PosDbContext _context;
        private readonly ItemJournalServices _journalServices;
        private readonly WareHouseServices _wareHouseServices;
        private readonly ItemServices _itemServices;
        private readonly CustomerServices _priceService;
        private readonly BPJornalServices _bPJornalServices;
        private readonly CustomerServices _customerServices;
        private readonly IConfiguration _config;

        public SalesServices(PosDbContext posDbContext, ItemJournalServices journalServices, WareHouseServices wareHouseServices, ItemServices itemServices, CustomerServices priceService, BPJornalServices bPJornalServices, CustomerServices customerServices, IConfiguration config)
        {
            _context = posDbContext;
            _journalServices = journalServices;
            _wareHouseServices = wareHouseServices;
            _itemServices = itemServices;
            _priceService = priceService;
            _bPJornalServices = bPJornalServices;
            _customerServices = customerServices;
            _config = config;
        }

        public List<OrderSaleDto> GetOrderSaleById(int id)
        {
            var result = GetBase(x => x.DocId == id).ToList();
            return result;
        }
        public List<OrderSaleDto> GetOrderSaleActive()
        {
            var result = GetBase(x => x.Complete == false).ToList();
            return result;
        }
        public List<OrderSaleDto> GetOrderSaleByDate(DateTime From, DateTime To)
        {
            var result = GetBase(x => x.DocDate.Date >= From && x.DocDate.Date <= To.Date).ToList();
            return result;
        }
        private List<OrderSaleDto> GetBase(Func<OrderSale, bool> condition)
        {
            var order = _context.OrderSale.Where(condition).ToList();
            var userId = order.Select(x => x.CreateBy).Distinct().ToList();
            var user = _context.User.Where(x => userId.Contains(x.UserId)).ToList();
            var whsCode = order.Select(x => x.WhsCode).Distinct().ToList();
            var whs = _context.WareHouse.Where(x => whsCode.Contains(x.WhsCode)).ToList();
            var orderId = order.Select(x => x.DocId).Distinct().ToList();
            var orderDetail = _context.OrderSaleDetail.Where(x => orderId.Contains(x.DocId)).ToList();
            var itemId = orderDetail.Select(x => x.ItemId).Distinct().ToList();
            var items = _context.Item.Where(x => itemId.Contains(x.ItemId)).ToList();
            var unit = orderDetail.Select(x => x.UnitOfMeasureId).Distinct().ToList();
            var units = _context.UnitOfMeasure.Where(x => unit.Contains(x.UnitOfMeasureId)).ToList();
            var payId = order.Select(x => x.PayConditionId).Distinct().ToList();
            var pays = _context.PayCondition.Where(x => payId.Contains(x.PayConditionId)).ToList();
            var sellerId = order.Select(x => x.SellerId).Distinct().ToList();
            var sellers = _context.Seller.Where(x => sellerId.Contains(x.SellerId)).ToList();

            var edetail = (from d in orderDetail
                           join i in items on d.ItemId equals i.ItemId
                           join w in whs on d.WhsCode equals w.WhsCode
                           join u in units on d.UnitOfMeasureId equals u.UnitOfMeasureId
                           where d.IsDelete == false
                           select new OrderSaleDetailDto
                           {
                               DocDetailId = d.DocDetailId,
                               DocId = d.DocId,
                               ItemId = i.ItemId,
                               ItemCode = i.ItemCode,
                               ItemName = i.ItemName,
                               Quantity = d.Quantity,
                               DueDate = d.DueDate,
                               Price = d.Price,
                               Cost = d.Cost,
                               WhsCode = w.WhsCode,
                               WhsName = w.WhsName,
                               UnitOfMeasureId = d.UnitOfMeasureId,
                               UnitOfMeasureName = u.UnitOfMeasureName,
                               LineTotal = d.LineTotal
                           }).ToList();

            var result = (from e in order
                          join d in edetail on e.DocId equals d.DocId into detail
                          join w in whs on e.WhsCode equals w.WhsCode
                          join u in user on e.CreateBy equals u.UserId
                          join p in pays on e.PayConditionId equals p.PayConditionId
                          join s in sellers on e.SellerId equals s.SellerId
                          select new OrderSaleDto
                          {
                              DocId = e.DocId,
                              CustomerId = e.CustomerId,
                              CustomerCode = e.CustomerCode,
                              CustomerName = e.CustomerName,
                              CustomerAddress = e.CustomerAddress,
                              CustomerRTN = e.CustomerRTN,
                              PayConditionId = e.PayConditionId,
                              PayConditionName = p.PayConditionName,
                              DocDate = e.DocDate,
                              DueDate = e.DueDate,
                              Canceled = e.Canceled,
                              Comment = e.Comment,
                              Reference = e.Reference,
                              WhsCode = e.WhsCode,
                              WhsName = w.WhsName,
                              SubTotal = e.SubTotal,
                              Tax = e.Tax,
                              Disccounts = e.Disccounts,
                              DiscountsTotal = e.DiscountsTotal,
                              DocTotal = e.DocTotal,
                              DocQty = detail.Sum(x => x.Quantity),
                              CreateBy = e.CreateBy,
                              CreateByName = u.Name,
                              Complete = e.Complete,
                              Detail = detail.ToList(),
                              TaxCustomer = e.Tax > 0 ? true : false,
                              SellerId = s.SellerId,
                              SellerName = s.SellerName,
                              PriceListId = e.PriceListId
                          }).ToList();
            return result.OrderByDescending(x => x.DocDate).ToList();
        }
        public List<OrderSaleDto> GetOrderSale()
        {
            var order = _context.OrderSale.ToList();
            var userId = order.Select(x => x.CreateBy).Distinct().ToList();
            var user = _context.User.Where(x => userId.Contains(x.UserId)).ToList();
            var whsCode = order.Select(x => x.WhsCode).Distinct().ToList();
            var whs = _context.WareHouse.Where(x => whsCode.Contains(x.WhsCode)).ToList();
            var orderId = order.Select(x => x.DocId).Distinct().ToList();
            var orderDetail = _context.OrderSaleDetail.Where(x => orderId.Contains(x.DocId)).ToList();
            var itemId = orderDetail.Select(x => x.ItemId).Distinct().ToList();
            var items = _context.Item.Where(x => itemId.Contains(x.ItemId)).ToList();
            var unit = orderDetail.Select(x => x.UnitOfMeasureId).Distinct().ToList();
            var units = _context.UnitOfMeasure.Where(x => unit.Contains(x.UnitOfMeasureId)).ToList();
            var payId = order.Select(x => x.PayConditionId).Distinct().ToList();
            var pays = _context.PayCondition.Where(x => payId.Contains(x.PayConditionId)).ToList();
            var sellerId = order.Select(x => x.SellerId).Distinct().ToList();
            var sellers = _context.Seller.Where(x => sellerId.Contains(x.SellerId)).ToList();
            var edetail = (from d in orderDetail
                           join i in items on d.ItemId equals i.ItemId
                           join w in whs on d.WhsCode equals w.WhsCode
                           join u in units on d.UnitOfMeasureId equals u.UnitOfMeasureId
                           where d.IsDelete == false
                           select new OrderSaleDetailDto
                           {
                               DocDetailId = d.DocDetailId,
                               DocId = d.DocId,
                               ItemId = i.ItemId,
                               ItemCode = i.ItemCode,
                               ItemName = i.ItemName,
                               Quantity = d.Quantity,
                               DueDate = d.DueDate,
                               Price = d.Price,
                               Cost = d.Cost,
                               WhsCode = w.WhsCode,
                               WhsName = w.WhsName,
                               UnitOfMeasureId = d.UnitOfMeasureId,
                               UnitOfMeasureName = u.UnitOfMeasureName,
                               LineTotal = d.LineTotal
                           }).ToList();

            var result = (from e in order
                          join d in edetail on e.DocId equals d.DocId into detail
                          join w in whs on e.WhsCode equals w.WhsCode
                          join u in user on e.CreateBy equals u.UserId
                          join p in pays on e.PayConditionId equals p.PayConditionId
                          join s in sellers on e.SellerId equals s.SellerId
                          select new OrderSaleDto
                          {
                              DocId = e.DocId,
                              CustomerId = e.CustomerId,
                              CustomerCode = e.CustomerCode,
                              CustomerName = e.CustomerName,
                              CustomerAddress = e.CustomerAddress,
                              CustomerRTN = e.CustomerRTN,
                              PayConditionId = e.PayConditionId,
                              PayConditionName = p.PayConditionName,
                              DocDate = e.DocDate,
                              DueDate = e.DueDate,
                              Canceled = e.Canceled,
                              Comment = e.Comment,
                              Reference = e.Reference,
                              WhsCode = e.WhsCode,
                              WhsName = w.WhsName,
                              SubTotal = e.SubTotal,
                              Tax = e.Tax,
                              Disccounts = e.Disccounts,
                              DiscountsTotal = e.DiscountsTotal,
                              DocTotal = e.DocTotal,
                              DocQty = detail.Sum(x => x.Quantity),
                              CreateBy = e.CreateBy,
                              CreateByName = u.Name,
                              Complete = e.Complete,
                              Detail = detail.ToList(),
                              TaxCustomer = e.Tax > 0 ? true : false,
                              SellerId = s.SellerId,
                              SellerName = s.SellerName,
                              PriceListId = e.PriceListId
                          }).ToList();
            return result.OrderByDescending(x => x.DocDate).ToList();
        }
        public List<OrderSaleDto> AddOrderSale(OrderSale request)
        {
            request.IsValid();
            _context.Database.BeginTransaction();
            try
            {
                // Obtén la hora actual del servidor (Oregón)
                DateTime serverTime = DateTime.Now;

                // Define la zona horaria de Honduras
                TimeZoneInfo hondurasTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central America Standard Time");
                // Asigna la hora de Honduras a request.DocDate
                request.DocDate = TimeZoneInfo.ConvertTime(serverTime, hondurasTimeZone);

                request.DocQty = request.Detail.Sum(x => x.Quantity);
                request.Complete = false;
                request.Canceled = false;
                _context.OrderSale.Add(request);
                _context.SaveChanges();
                _context.Database.CommitTransaction();
            }
            catch (Exception ex)
            {
                _context.Database.RollbackTransaction();
                throw new Exception(ex.Message);
            }
            return GetOrderSaleById(request.DocId);
        }
        public List<OrderSaleDto> EditOrderSale(OrderSale request)
        {
            request.IsValid();
            var currentOrder = _context.OrderSale.Where(x => x.DocId == request.DocId).FirstOrDefault();
            if (currentOrder == null) throw new Exception("No existe esta orden, comuniquese con el administrador del sistema.");
            _context.Database.BeginTransaction();
            try
            {
                currentOrder.CustomerCode = request.CustomerCode;
                currentOrder.CustomerName = request.CustomerName;
                currentOrder.CustomerId = request.CustomerId;
                currentOrder.WhsCode = request.WhsCode;
                currentOrder.Canceled = request.Canceled;
                currentOrder.Comment = request.Comment;
                currentOrder.Reference = request.Reference;
                currentOrder.SubTotal = request.SubTotal;
                currentOrder.Tax = request.Tax;
                currentOrder.Disccounts = request.Disccounts;
                currentOrder.DiscountsTotal = request.DiscountsTotal;
                currentOrder.DocTotal = request.DocTotal;
                currentOrder.DocQty = request.DocQty;
                currentOrder.Complete = request.Complete;
                currentOrder.PayConditionId = request.PayConditionId;
                // Borrar todas las líneas existentes
                var existingLines = _context.OrderSaleDetail.Where(x => x.DocId == currentOrder.DocId);
                _context.OrderSaleDetail.RemoveRange(existingLines);

                // Agregar todas las líneas como nuevas
                foreach (var detail in request.Detail)
                {
                    detail.DocId = currentOrder.DocId;
                    detail.DocDetailId = 0; 
                    detail.WhsCode = currentOrder.WhsCode;
                    _context.OrderSaleDetail.Add(detail);
                }
                _context.OrderSale.Update(currentOrder);
                _context.SaveChanges();
                _context.Database.CommitTransaction();
            }
            catch (Exception ex)
            {
                _context.Database.RollbackTransaction();
                throw new Exception(ex.Message);
            }
            return GetOrderSaleById(request.DocId);
        }
        public List<OrderSaleDto> CanceledOrderSale(int docId)
        {
            var currentOrder = _context.OrderSale.Where(x => x.DocId == docId).FirstOrDefault();
            if (currentOrder == null) throw new Exception("No existe esta orden, comuniquese con el administrador del sistema.");
            _context.Database.BeginTransaction();
            try
            {
                currentOrder.Canceled = true;
                currentOrder.Complete = true;
                _context.OrderSale.Update(currentOrder);
                _context.SaveChanges();
                _context.Database.CommitTransaction();
            }
            catch (Exception ex)
            {
                _context.Database.RollbackTransaction();
                throw new Exception(ex.Message);
            }
            return GetOrderSaleById(docId);
        }
        public List<OrderSaleDto> CompleteOrderSale(int docId, bool status)
        {
            var currentOrder = _context.OrderSale.Where(x => x.DocId == docId).FirstOrDefault();
            if (currentOrder == null) throw new Exception("No existe esta orden, comuniquese con el administrador del sistema.");
            try
            {
                currentOrder.Complete = status;
                _context.OrderSale.Update(currentOrder);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
            return GetOrderSaleById(docId);
        }
        //Invoice
        public List<InvoiceSaleDto> GetInvoiceSaleById(int id)
        {
            var result = GetBaseInvoice(x => x.DocId == id).ToList();
            return result;
        }
        public List<InvoiceSaleDto> GetInvoiceSaleActive()
        {
            var result = GetBaseInvoice(x => x.Complete == false).ToList();
            return result;
        }
        public List<InvoiceSaleDto> GetInvoiceSaleActiveCustomer(int idCustomer)
        {
            string conec = _config["connectionStrings:dbpos"];
            using (var connection = new SqlConnection(conec))
            {
                connection.Open();
                var result = connection.Query<InvoiceSaleDto>(
                    "GetInvoiceSaleActiveCustomer",
                    new { CustomerId = idCustomer },
                    commandType: CommandType.StoredProcedure
                );

                // Fetch details for each invoice
                foreach (var invoice in result)
                {
                    invoice.DetailDto = connection.Query<InvoiceSaleDetailDto>(
                        "SELECT d.DocDetailId, d.DocId, item.ItemId, item.ItemCode, item.ItemName, " +
                        "d.Quantity, d.DueDate, d.Price, d.Cost, w.WhsCode, w.WhsName, " +
                        "d.UnitOfMeasureId, uom.UnitOfMeasureName, d.LineTotal " +
                        "FROM InvoiceSaleDetail d " +
                        "JOIN Item item ON d.ItemId = item.ItemId " +
                        "JOIN WareHouse w ON d.WhsCode = w.WhsCode " +
                        "JOIN UnitOfMeasure uom ON d.UnitOfMeasureId = uom.UnitOfMeasureId " +
                        "WHERE d.DocId = @DocId AND d.IsDelete = 0",
                        new { DocId = invoice.DocId }
                    ).ToList();
                }
                
                return result.ToList();
            }
        }
        public List<InvoiceSaleDto> GetInvoiceSaleActiveSeller(int idSeller)
        {
            string conec = _config["connectionStrings:dbpos"];
            using (var connection = new SqlConnection(conec))
            {
                connection.Open();
                var result = connection.Query<InvoiceSaleDto>(
                "GetInvoiceSaleActive",
                    commandType: CommandType.StoredProcedure
                );

                //// Fetch details for each invoice
                //foreach (var invoice in result)
                //{
                //    invoice.DetailDto = connection.Query<InvoiceSaleDetailDto>(
                //        "SELECT d.DocDetailId, d.DocId, item.ItemId, item.ItemCode, item.ItemName, " +
                //        "d.Quantity, d.DueDate, d.Price, d.Cost, w.WhsCode, w.WhsName, " +
                //        "d.UnitOfMeasureId, uom.UnitOfMeasureName, d.LineTotal " +
                //        "FROM InvoiceSaleDetail d " +
                //        "JOIN Item item ON d.ItemId = item.ItemId " +
                //        "JOIN WareHouse w ON d.WhsCode = w.WhsCode " +
                //        "JOIN UnitOfMeasure uom ON d.UnitOfMeasureId = uom.UnitOfMeasureId " +
                //        "WHERE d.DocId = @DocId AND d.IsDelete = 0",
                //        new { DocId = invoice.DocId }
                //    ).ToList();
                //}

                return result.ToList();
            }
        }
        public List<InvoiceSaleDto> GetInvoiceSaleByDate(DateTime From, DateTime To)
        {
            var invoice = _context.InvoiceSale.Where(x => x.DocDate.Date >= From.Date && x.DocDate.Date <= To.Date.Date).ToList();
            var userId = invoice.Select(x => x.CreateBy).Distinct().ToList();
            var user = _context.User.Where(x => userId.Contains(x.UserId)).ToList();
            var whsCode = invoice.Select(x => x.WhsCode).Distinct().ToList();
            var whs = _context.WareHouse.Where(x => whsCode.Contains(x.WhsCode)).ToList();
            var invoiceId = invoice.Select(x => x.DocId).Distinct().ToList();
            var invoiceDetail = _context.InvoiceSaleDetail.Where(x => invoiceId.Contains(x.DocId)).ToList();
            var itemId = invoiceDetail.Select(x => x.ItemId).Distinct().ToList();
            var items = _context.Item.Where(x => itemId.Contains(x.ItemId)).ToList();
            var unit = invoiceDetail.Select(x => x.UnitOfMeasureId).Distinct().ToList();
            var units = _context.UnitOfMeasure.Where(x => unit.Contains(x.UnitOfMeasureId)).ToList();
            var payId = invoice.Select(x => x.PayConditionId).Distinct().ToList();
            var pays = _context.PayCondition.Where(x => payId.Contains(x.PayConditionId)).ToList();
            var sellerId = invoice.Select(x => x.SellerId).Distinct().ToList();
            var sellers = _context.Seller.Where(x => sellerId.Contains(x.SellerId)).ToList();

            var edetail = (from d in invoiceDetail
                           join i in items on d.ItemId equals i.ItemId
                           join w in whs on d.WhsCode equals w.WhsCode
                           join u in units on d.UnitOfMeasureId equals u.UnitOfMeasureId
                           where d.IsDelete == false
                           select new InvoiceSaleDetailDto
                           {
                               DocDetailId = d.DocDetailId,
                               DocId = d.DocId,
                               ItemId = i.ItemId,
                               ItemCode = i.ItemCode,
                               ItemName = i.ItemName,
                               Quantity = d.Quantity,
                               DueDate = d.DueDate,
                               Price = d.Price,
                               Cost = d.Cost,
                               WhsCode = w.WhsCode,
                               WhsName = w.WhsName,
                               UnitOfMeasureId = d.UnitOfMeasureId,
                               UnitOfMeasureName = u.UnitOfMeasureName,
                               LineTotal = d.LineTotal,
                           }).ToList();

            var result = (from e in invoice
                          join d in edetail on e.DocId equals d.DocId into detail
                          join w in whs on e.WhsCode equals w.WhsCode
                          join u in user on e.CreateBy equals u.UserId
                          join p in pays on e.PayConditionId equals p.PayConditionId
                          join s in sellers on e.SellerId equals s.SellerId
                          select new InvoiceSaleDto
                          {
                              DocId = e.DocId,
                              CustomerId = e.CustomerId,
                              CustomerCode = e.CustomerCode,
                              CustomerName = e.CustomerName,
                              CustomerAddress = e.CustomerAddress,
                              CustomerRTN = e.CustomerRTN,
                              PayConditionId = e.PayConditionId,
                              PayConditionName = p.PayConditionName,
                              DocDate = e.DocDate,
                              DueDate = e.DueDate,
                              Canceled = e.Canceled,
                              Comment = e.Comment,
                              Reference = e.Reference,
                              WhsCode = e.WhsCode,
                              WhsName = w.WhsName,
                              SubTotal = e.SubTotal,
                              Tax = e.Tax,
                              Disccounts = e.Disccounts,
                              DiscountsTotal = e.DiscountsTotal,
                              DocTotal = e.DocTotal,
                              DocQty = detail.Sum(x => x.Quantity),
                              CreateBy = e.CreateBy,
                              CreateByName = u.Name,
                              Complete = e.Complete,
                              DetailDto = detail.ToList(),
                              TaxCustomer = e.Tax > 0 ? true : false,
                              SellerId = s.SellerId,
                              SellerName = s.SellerName,
                              PriceListId = e.PriceListId,
                              InvoiceFiscalNo = e.InvoiceFiscalNo,
                              Establishment = e.Establishment,
                              Point = e.Point,
                              Type = e.Type,
                              Cai = e.Cai,
                              LimitIssue = e.LimitIssue,
                              AuthorizedRangeFrom = e.AuthorizedRangeFrom,
                              AuthorizedRangeTo = e.AuthorizedRangeTo,
                              CorrelativeId = e.CorrelativeId,
                              Balance = e.Balance,
                              PaidToDate = e.PaidToDate
                          }).ToList();
            return result.OrderByDescending(x => x.DocId).ToList();    
        }
        private List<InvoiceSaleDto> GetBaseInvoice(Expression<Func<InvoiceSale, bool>> condition)
        {
            return _context.InvoiceSale
                .Include(x => x.Detail)
                    .ThenInclude(x => x.Items)
                .Include(x => x.Detail)
                    .ThenInclude(x => x.WareHouse)
                .Include(x => x.Detail)
                    .ThenInclude(x => x.UnitOfMeasure)
                .Include(x => x.WareHouse)
                .Include(x => x.User)
                .Include(x => x.PayCondition)
                .Include(x => x.Seller)
                .Where(condition)
                .Where(x => x.Detail.Any(d => !d.IsDelete))
                .AsNoTracking()
                .Select(e => new InvoiceSaleDto
                {
                    DocId = e.DocId,
                    CustomerId = e.CustomerId,
                    CustomerCode = e.CustomerCode,
                    CustomerName = e.CustomerName,
                    CustomerAddress = e.CustomerAddress,
                    CustomerRTN = e.CustomerRTN,
                    PayConditionId = e.PayConditionId,
                    PayConditionName = e.PayCondition.PayConditionName,
                    DocDate = e.DocDate,
                    DueDate = e.DueDate,
                    Canceled = e.Canceled,
                    Comment = e.Comment,
                    Reference = e.Reference,
                    WhsCode = e.WhsCode,
                    WhsName = e.WareHouse.WhsName,
                    SubTotal = e.SubTotal,
                    Tax = e.Tax,
                    Disccounts = e.Disccounts,
                    DiscountsTotal = e.DiscountsTotal,
                    DocTotal = e.DocTotal,
                    DocQty = e.Detail.Where(d => !d.IsDelete).Sum(x => x.Quantity),
                    CreateBy = e.CreateBy,
                    CreateByName = e.User.Name,
                    Complete = e.Complete,
                    DetailDto = e.Detail.Where(d => !d.IsDelete).Select(d => new InvoiceSaleDetailDto
                    {
                        DocDetailId = d.DocDetailId,
                        DocId = d.DocId,
                        ItemId = d.Items.ItemId,
                        ItemCode = d.Items.ItemCode,
                        ItemName = d.Items.ItemName,
                        Quantity = d.Quantity,
                        DueDate = d.DueDate,
                        Price = d.Price,
                        Cost = d.Cost,
                        WhsCode = d.WareHouse.WhsCode,
                        WhsName = d.WareHouse.WhsName,
                        UnitOfMeasureId = d.UnitOfMeasureId,
                        UnitOfMeasureName = d.UnitOfMeasure.UnitOfMeasureName,
                        LineTotal = d.LineTotal
                    }).ToList(),
                    TaxCustomer = e.Tax > 0,
                    SellerId = e.Seller.SellerId,
                    SellerName = e.Seller.SellerName,
                    PriceListId = e.PriceListId,
                    InvoiceFiscalNo = e.InvoiceFiscalNo,
                    Establishment = e.Establishment,
                    Point = e.Point,
                    Type = e.Type,
                    Cai = e.Cai,
                    LimitIssue = e.LimitIssue,
                    AuthorizedRangeFrom = e.AuthorizedRangeFrom,
                    AuthorizedRangeTo = e.AuthorizedRangeTo,
                    CorrelativeId = e.CorrelativeId,
                    Balance = e.Balance,
                    PaidToDate = e.PaidToDate,
                    Uuid = e.Uuid
                })
                .OrderByDescending(x => x.DocDate)
                .ToList();
        }
        public List<InvoiceSaleDto> GetInvoiceSale()
        {
            var invoice = _context.InvoiceSale.ToList();
            var userId = invoice.Select(x => x.CreateBy).Distinct().ToList();
            var user = _context.User.Where(x => userId.Contains(x.UserId)).ToList();
            var whsCode = invoice.Select(x => x.WhsCode).Distinct().ToList();
            var whs = _context.WareHouse.Where(x => whsCode.Contains(x.WhsCode)).ToList();
            var invoiceId = invoice.Select(x => x.DocId).Distinct().ToList();
            var invoiceDetail = _context.InvoiceSaleDetail.Where(x => invoiceId.Contains(x.DocId)).ToList();
            var itemId = invoiceDetail.Select(x => x.ItemId).Distinct().ToList();
            var items = _context.Item.Where(x => itemId.Contains(x.ItemId)).ToList();
            var unit = invoiceDetail.Select(x => x.UnitOfMeasureId).Distinct().ToList();
            var units = _context.UnitOfMeasure.Where(x => unit.Contains(x.UnitOfMeasureId)).ToList();
            var payId = invoice.Select(x => x.PayConditionId).Distinct().ToList();
            var pays = _context.PayCondition.Where(x => payId.Contains(x.PayConditionId)).ToList();
            var sellerId = invoice.Select(x => x.SellerId).Distinct().ToList();
            var sellers = _context.Seller.Where(x => sellerId.Contains(x.SellerId)).ToList();

            var edetail = (from d in invoiceDetail
                           join i in items on d.ItemId equals i.ItemId
                           join w in whs on d.WhsCode equals w.WhsCode
                           join u in units on d.UnitOfMeasureId equals u.UnitOfMeasureId
                           where d.IsDelete == false
                           select new InvoiceSaleDetailDto
                           {
                               DocDetailId = d.DocDetailId,
                               DocId = d.DocId,
                               ItemId = i.ItemId,
                               ItemCode = i.ItemCode,
                               ItemName = i.ItemName,
                               Quantity = d.Quantity,
                               DueDate = d.DueDate,
                               Price = d.Price,
                               Cost = d.Cost,
                               WhsCode = w.WhsCode,
                               WhsName = w.WhsName,
                               UnitOfMeasureId = d.UnitOfMeasureId,
                               UnitOfMeasureName = u.UnitOfMeasureName,
                               LineTotal = d.LineTotal
                           }).ToList();

            var result = (from e in invoice
                          join d in edetail on e.DocId equals d.DocId into detail
                          join w in whs on e.WhsCode equals w.WhsCode
                          join u in user on e.CreateBy equals u.UserId
                          join p in pays on e.PayConditionId equals p.PayConditionId
                          join s in sellers on e.SellerId equals s.SellerId
                          select new InvoiceSaleDto
                          {
                              DocId = e.DocId,
                              CustomerId = e.CustomerId,
                              CustomerCode = e.CustomerCode,
                              CustomerName = e.CustomerName,
                              CustomerAddress = e.CustomerAddress,
                              CustomerRTN = e.CustomerRTN,
                              PayConditionId = e.PayConditionId,
                              PayConditionName = p.PayConditionName,
                              DocDate = e.DocDate,
                              DueDate = e.DueDate,
                              Canceled = e.Canceled,
                              Comment = e.Comment,
                              Reference = e.Reference,
                              WhsCode = e.WhsCode,
                              WhsName = w.WhsName,
                              SubTotal = e.SubTotal,
                              Tax = e.Tax,
                              Disccounts = e.Disccounts,
                              DiscountsTotal = e.DiscountsTotal,
                              DocTotal = e.DocTotal,
                              DocQty = detail.Sum(x => x.Quantity),
                              CreateBy = e.CreateBy,
                              CreateByName = u.Name,
                              Complete = e.Complete,
                              DetailDto = detail.ToList(),
                              TaxCustomer = e.Tax > 0 ? true : false,
                              SellerId = s.SellerId,
                              SellerName = s.SellerName,
                              PriceListId = e.PriceListId,
                              InvoiceFiscalNo = e.InvoiceFiscalNo,
                              Establishment = e.Establishment,
                              Point = e.Point,
                              Type = e.Type,
                              Cai = e.Cai,
                              LimitIssue = e.LimitIssue,
                              AuthorizedRangeFrom = e.AuthorizedRangeFrom,
                              AuthorizedRangeTo = e.AuthorizedRangeTo,
                              CorrelativeId = e.CorrelativeId
                          }).ToList();
            return result.OrderByDescending(x => x.DocDate).ToList();
        }
        public List<InvoiceSaleDto> AddInvoiceSale(InvoiceSaleDto request)
        {
            request.SubTotal = request.Detail.Sum(x => (x.Quantity * x.Price));
            request.DocTotal = request.SubTotal + request.Tax - request.DiscountsTotal;
            request.IsValid();
            var uuid = _context.InvoiceSale.Where(x => x.Uuid == request.Uuid).FirstOrDefault();
            if (uuid != null) throw new Exception("Error: Esta factura ya existe en la base de datos. UUID");
            _context.Database.BeginTransaction();
            try
            {
                request.DocId = 0;
                DateTime fechaPorDefecto = DateTime.MinValue;
                var customer = _context.Customer
                    .Where(x=> x.CustomerId == request.CustomerId)
                    .Select(x=> new {x.LimitInvoiceCredit, x.Balance, x.CreditLine}).FirstOrDefault();
                if(request.PayConditionId != 1)
                {
                    if ((customer.Balance + request.DocTotal) > customer.CreditLine && request.PayConditionId != 1) throw new Exception("Error: No se puede crear la factura, ha excedido el límite de crédito de: L." + customer.CreditLine.ToString("N") + ".\nActualmente este cliente tiene un saldo de: L." + customer.Balance.ToString("N") + ".\nCrédito sobregirado por: L." + (customer.CreditLine - (customer.Balance + request.DocTotal)).ToString("N") + ".");
                    var sale = _context.InvoiceSale.Where(x => x.Balance > 0 && x.CustomerId == request.CustomerId && x.Canceled== false).Count();
                    if (sale >= customer.LimitInvoiceCredit) throw new Exception("Error: No se puede crear la factura. Este cliente tiene " + sale + " facturas con saldo pendiente. Solo puede tener " + sale + " facturas pendientes de pago.");
                }
               
                var point = _context.SarPointSale.Select(x=> new {x.PointSaleId, x.BranchId}).FirstOrDefault();
                var correlative = _context.SarCorrelative
                                .Where(x => x.CorrelativeId == request.CorrelativeId).FirstOrDefault();
                if (correlative == null) throw new Exception("Error: No se ha agregado un correlativo para este punto de facturación");
                if (request.Detail.Count==0) throw new Exception("Error: No se puede agregar este documento, falton datos de articulos, favor contacte al administrador.");
                if (correlative.DateLimit.Date <= DateTime.Now.Date) throw new Exception("Error: La fecha limite para el correlativo actual ya expiro, por favor agregar uno nuevo");
                if (correlative.CurrentCorrelative >= correlative.AuthorizeRangeTo) throw new Exception("Error: Ya se cumplio el numero limite para emitir facturas, por favor solicitar uno nuevo");
                correlative.CurrentCorrelative += 1;
                if(request.InvoiceFiscalNo == null)
                    request.InvoiceFiscalNo = correlative.CurrentCorrelative.ToString().PadLeft(8, ' ').Replace(' ', '0'); //calcular falta hacer el metodo;
                request.Point = point.PointSaleId;
                request.Establishment = point.BranchId;
                request.Type = "01";
                request.LimitIssue = correlative.DateLimit.Date;
                request.AuthorizedRangeFrom = correlative.AuthorizeRangeFrom.ToString().PadLeft(8, ' ').Replace(' ', '0');
                request.AuthorizedRangeTo = correlative.AuthorizeRangeTo.ToString().PadLeft(8, ' ').Replace(' ', '0');
                request.Cai = correlative.Cai;
                // Obtén la hora actual del servidor (Oregón)
                DateTime serverTime = DateTime.Now;

                // Define la zona horaria de Honduras
                TimeZoneInfo hondurasTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central America Standard Time");
                // Asigna la hora de Honduras a request.DocDat
                request.DocDate = request.DocDate != fechaPorDefecto? request.DocDate: TimeZoneInfo.ConvertTime(serverTime, hondurasTimeZone);
                if (request.Offline)
                    request.DocDate = TimeZoneInfo.ConvertTime(serverTime, hondurasTimeZone);
                request.DocQty = request.Detail.Sum(x => x.Quantity);
                request.Complete = false;
                request.Canceled = false;
                request.Balance = request.DocTotal;
                request.Detail.ForEach(x => x.DocDetailId = 0);
  
                if (request.PayConditionId == 1)
                    request.Complete = true;
                _context.InvoiceSale.Add(request);
                _context.SaveChanges();
                //Rebajamos inventario
                var journal = request.Detail.Select(x => new ItemJournal
                {
                    ItemId = x.ItemId,
                    WhsCode = x.WhsCode,
                    Quantity = x.Quantity * -1,
                    Price = x.Price,
                    TransValue = (x.Price * x.Quantity) * -1,
                    Documents = "Factura de Venta",
                    DocumentReferent = request.DocId,
                    CreateBy = request.CreateBy,
                    CreateDate = request.DocDate
                }).ToList();
                _journalServices.AddLinesJournal(journal);
                //Actualizamos el inventario del almacen
                var warehouse = request.Detail.Select(x => new ItemWareHouse
                {
                    ItemId = x.ItemId,
                    WhsCode = x.WhsCode,
                    Stock = x.Quantity * -1,
                    AvgPrice = x.Price,
                    CreateDate = request.DocDate,
                    DueDate = x.DueDate
                }).ToList();

                warehouse.ForEach(x => _wareHouseServices.UpdateItemWareHouse(x));
                warehouse.ForEach(x => _itemServices.UpdateStockItem(x.ItemId));

                //Aumentamos el saldo por cobrar
                var bpjournal = new BPJornal
                {
                    BpId = request.CustomerId,
                    DocId = request.DocId,
                    BpType = "C",
                    TransValue = request.DocTotal,
                    Documents = "Factura de venta",
                    DocumentReferent = request.DocId,
                    CreateBy = request.CreateBy,
                    CreateDate = request.DocDate,
                    UUID = request.Uuid,
                };
                _bPJornalServices.AddLineBPJournal(bpjournal);
                _customerServices.UpdateBalanceCustomer(request.CustomerId, request.DocTotal);
                //Si viene de un pedido, cerramos ese pedido
                if (request.DocReference != 0)
                    this.CompleteOrderSale(request.DocReference, true);
                //Validamos si es factura contado y registramos el pago
                if(request.PayConditionId == 1)
                {
                    request.Payment.CustomerId = request.CustomerId;
                    request.Payment.CustomerCode = request.CustomerCode;
                    request.Payment.CustomerName = request.CustomerName;
                    request.Payment.PayConditionId = request.PayConditionId;
                    request.Payment.Reference = request.DocId.ToString();
                    request.Payment.DocDate = request.DocDate;
                    request.Payment.Comment = "Factura de contado: " + request.InvoiceFiscalNo;
                    request.Payment.DocTotal = request.DocTotal;
                    request.Payment.CreateBy = request.CreateBy;
                    request.Payment.CashSum = request.DocTotal;
                    request.Payment.Uuid = request.Uuid;
                    var paymentDetail = new PaymentSaleDetail
                    {
                        InvoiceId = request.DocId,
                        InvoiceDate = request.DocDate,
                        InvoiceReference = request.DocId.ToString(),
                        DueDate = request.DueDate,
                        SubTotal = request.SubTotal,
                        TaxTotal = request.Tax,
                        DiscountTotal = request.DiscountsTotal,
                        LineTotal = request.DocTotal,
                        SumApplied = request.Payment.DocTotal
                    };
                    request.Payment.Detail.Add(paymentDetail);
                    AddPaymentSaleCounted(request.Payment);
                }
                _context.Database.CommitTransaction();
            }
            catch (Exception ex)
            {
                _context.Database.RollbackTransaction();
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
            return GetInvoiceSaleByDate(request.DocDate, request.DocDate);
        }
        public List<InvoiceSaleDto> EditInvoiceSale(InvoiceSale request)
        {
            request.IsValid();
            var currentInvoice = _context.InvoiceSale.Where(x => x.DocId == request.DocId).FirstOrDefault();
            if (currentInvoice == null) throw new Exception("Error: No existe esta orden, comuniquese con el administrador del sistema.");
            if (currentInvoice.PaidToDate > 0) throw new Exception("Error: No puede editar esta factura, ya que tiene un pago aplicado.");
            _context.Database.BeginTransaction();
            try
            {
                currentInvoice.CustomerCode = request.CustomerCode;
                currentInvoice.CustomerName = request.CustomerName;
                currentInvoice.CustomerId = request.CustomerId;
                currentInvoice.WhsCode = request.WhsCode;
                currentInvoice.Canceled = request.Canceled;
                currentInvoice.Comment = request.Comment;
                currentInvoice.Reference = request.Reference;
                currentInvoice.SubTotal = request.SubTotal;
                currentInvoice.Tax = request.Tax;
                currentInvoice.Disccounts = request.Disccounts;
                currentInvoice.DiscountsTotal = request.DiscountsTotal;
                currentInvoice.DocTotal = request.DocTotal;
                currentInvoice.DocQty = request.DocQty;
                currentInvoice.Complete = request.Complete;
                var currentDetail = _context.InvoiceSaleDetail.Where(x => x.DocId == currentInvoice.DocId).ToList();
                var newLine = request.Detail.Where(x => x.DocDetailId == 0).ToList();
                //Agregamos las nuevas lineas
                _context.InvoiceSaleDetail.AddRange(newLine);
                //Actualizamos las existente
                currentDetail.ForEach((item) =>
                {
                    var itemToUpdate = request.Detail.Where(x => x.DocDetailId == item.DocDetailId && item.DocDetailId != 0).FirstOrDefault();
                    if (itemToUpdate != null)
                    {
                        item.ItemId = itemToUpdate.ItemId;
                        item.Quantity = itemToUpdate.Quantity;
                        item.DueDate = itemToUpdate.DueDate;
                        item.Price = itemToUpdate.Price;
                        item.WhsCode = currentInvoice.WhsCode;
                        item.UnitOfMeasureId = itemToUpdate.UnitOfMeasureId;
                        item.LineTotal = itemToUpdate.LineTotal;
                        item.IsDelete = itemToUpdate.IsDelete;
                    }

                });
                _context.InvoiceSaleDetail.UpdateRange(currentDetail);
                _context.InvoiceSale.Update(currentInvoice);
                _context.SaveChanges();
                _context.Database.CommitTransaction();
            }
            catch (Exception ex)
            {
                _context.Database.RollbackTransaction();
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
            return GetInvoiceSaleById(request.DocId);
        }
        public List<InvoiceSaleDto> CanceledInvoiceSale(int docId)
        {
            var currentInvoice = _context.InvoiceSale.Where(x => x.DocId == docId).FirstOrDefault();
            var currentDetail = _context.InvoiceSaleDetail.Where(x => x.DocId == docId).ToList();
            if (currentInvoice == null) throw new Exception("No existe esta orden, comuniquese con el administrador del sistema.");
            var liquidation = _context.LiquidationDetail.Where(x => x.DocNum == docId && x.DocType.Contains("Factura")).FirstOrDefault();
            if (liquidation != null)
            {
                var liquitationStatus = _context.Liquidation.Where(x => x.IdLiquidation == liquidation.LiquidationId).FirstOrDefault();
                if (liquitationStatus.Active) throw new Exception("No puede cancelar esta factura, porque ya esta liquidada. Anule primero la liquidacion.");
            }          
            if(currentInvoice.PaidToDate>0) throw new Exception("No puede cancelar esta factura, ya que tiene un pago aplicado. Anule el pago primero.");
            _context.Database.BeginTransaction();
            try
            {
                currentInvoice.Canceled = true;
                currentInvoice.Complete = true;
                currentInvoice.Balance = 0;
                _context.InvoiceSale.Update(currentInvoice);
                _context.SaveChanges();
                //Aumentamos inventario rebajado
                var journal = currentDetail.Select(x => new ItemJournal
                {
                    ItemId = x.ItemId,
                    WhsCode = x.WhsCode,
                    Quantity = x.Quantity,
                    Price = x.Price,
                    TransValue = x.Price * x.Quantity,
                    Documents = "Factura de Venta -Anulacion",
                    DocumentReferent = currentInvoice.DocId,
                    CreateBy = currentInvoice.CreateBy,
                    CreateDate = currentInvoice.DocDate

                }).ToList();
                _journalServices.AddLinesJournal(journal);
                //Actualizamos el inventario del almacen
                var warehouse = currentDetail.Select(x => new ItemWareHouse
                {
                    ItemId = x.ItemId,
                    WhsCode = x.WhsCode,
                    Stock = x.Quantity,
                    AvgPrice = x.Cost,
                    CreateDate = DateTime.Now,
                    DueDate = x.DueDate
                }).ToList();
                warehouse.ForEach(x => _wareHouseServices.UpdateItemWareHouse(x));
                warehouse.ForEach(x => _itemServices.UpdateStockItem(x.ItemId));
                //warehouse.ForEach(x => _priceService.UpdatePriceListDetail(x.ItemId, x.AvgPrice));
                //Disminuimos el saldo por pagar
                var bpjournal = new BPJornal
                {
                    BpId = currentInvoice.CustomerId,
                    DocId = currentInvoice.DocId,
                    BpType = "C",
                    TransValue = currentInvoice.DocTotal * -1,
                    Documents = "Factura de Venta - Anulación",
                    DocumentReferent = currentInvoice.DocId,
                    CreateBy = currentInvoice.CreateBy,
                    CreateDate = DateTime.Now,
                    UUID = currentInvoice.Uuid+"AnulaciónFactura",
                };
                _bPJornalServices.AddLineBPJournal(bpjournal);
                _customerServices.UpdateBalanceCustomer(currentInvoice.CustomerId, currentInvoice.DocTotal * -1);
                //Si viene de un pedido, abrimos ese pedido
                if (currentInvoice.DocReference != 0)
                    this.CompleteOrderSale(currentInvoice.DocReference, false);
                _context.Database.CommitTransaction();

            }
            catch (Exception ex)
            {
                _context.Database.RollbackTransaction();
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
            return GetInvoiceSaleById(docId);
        }
        public List<InvoiceSaleDto> CompleteInvoiceSale(int docId, decimal sumApplied)
        {
            var currentInvoice = _context.InvoiceSale.Where(x => x.DocId == docId).FirstOrDefault();
            if (currentInvoice == null) throw new Exception("No existe esta orden, comuniquese con el administrador del sistema.");
            try
            {
                currentInvoice.PaidToDate = currentInvoice.PaidToDate + sumApplied;
                currentInvoice.Balance = currentInvoice.Balance - sumApplied;
                if (currentInvoice.Balance <= 0)
                    currentInvoice.Complete = true;
                else currentInvoice.Complete = false;
                _context.InvoiceSale.Update(currentInvoice);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return GetInvoiceSaleById(docId);
        }

        public async Task<List<InvoiceSaleDto>> CompleteInvoiceSaleAsync(int docId, decimal sumApplied)
        {
            var currentInvoice = await _context.InvoiceSale
                .FirstOrDefaultAsync(x => x.DocId == docId);

            if (currentInvoice == null)
                throw new Exception("No existe esta orden, comuníquese con el administrador del sistema.");

            try
            {
                currentInvoice.PaidToDate += sumApplied;
                currentInvoice.Balance -= sumApplied;
                currentInvoice.Complete = currentInvoice.Balance <= 0;

                _context.InvoiceSale.Update(currentInvoice);
                await _context.SaveChangesAsync();

                return Task.FromResult(GetInvoiceSaleById(docId)).Result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<InvoiceSaleDto> CompleteInvoiceSaleFromPayment(int docId, decimal sumApplied, bool isOffline, string uuid)
        {
            var currentInvoice = _context.InvoiceSale
                .FirstOrDefault(x => isOffline ? x.Uuid == uuid : x.DocId == docId);

            if (currentInvoice == null)
                throw new Exception("No se ha sincronizado la factura, intente sincronizar el pago manualmente, despues que sincronice la factura.");

            try
            {
                currentInvoice.PaidToDate += sumApplied;
                currentInvoice.Balance -= sumApplied;
                currentInvoice.Complete = currentInvoice.Balance <= 0;

                _context.InvoiceSale.Update(currentInvoice);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al actualizar la factura: {ex.Message}", ex);
            }

            return GetInvoiceSaleById(docId);
        }
        public List<CustomerAccountDto> GetCustomerAccountBalance()
        {
            var currentDate = DateTime.Now.Date;
            var sale = _context.InvoiceSale.Where(x => x.Balance > 0 && x.Canceled==false).ToList();
            var customerId = sale.Select(x => x.CustomerId).Distinct().ToList();
            var customer = _context.Customer.Where(x => customerId.Contains(x.CustomerId)).ToList();
            var sellerId = sale.Select(x => x.SellerId).Distinct().ToList();
            var sellers = _context.Seller.Where(x => sellerId.Contains(x.SellerId)).ToList();
            var payId = sale.Select(x => x.PayConditionId).Distinct().ToList();
            var paycondition = _context.PayCondition.Where(x => payId.Contains(x.PayConditionId)).ToList();
            var customerFrecuency = _context.CustomerFrequency.ToList();
            var response = (from p in sale
                            join s in sellers on p.SellerId equals s.SellerId
                            join c in customer on p.CustomerId equals c.CustomerId
                            join pay in paycondition on p.PayConditionId equals pay.PayConditionId
                            join f in customerFrecuency on c.FrequencyId equals f.Id
                            orderby p.CustomerId
                            select new CustomerAccountDto
                            {
                                CustomerId = p.CustomerId,
                                CustomerCode = p.CustomerCode,
                                CustomerName = p.CustomerName,
                                Frecuency = f.FrequencyName,
                                SellerName = s.SellerName,
                                SellerId = s.SellerId,
                                PayConditionName = pay.PayConditionName,
                                PayConditionId = pay.PayConditionId,
                                Uuid = p.Uuid,
                                SubTotal =p.SubTotal,
                                Tax = p.Tax,
                                DocDate = p.DocDate,
                                DiscountsTotal = p.DiscountsTotal,
                                InvoiceNumber = p.DocId,
                                DocTotal = p.DocTotal,
                                PaidToDate = p.PaidToDate,
                                Balance = p.Balance,
                                DueDate = p.DueDate,
                                UnexpiredBalance = p.DueDate > currentDate ? p.Balance : 0,
                                BalanceDue = p.DueDate < currentDate ? p.Balance : 0,
                                BalanceAt30Days = (p.DueDate >= currentDate.AddDays(-30) && p.DueDate <= currentDate) ? p.Balance : 0,
                                BalanceFrom31To60Days = (p.DueDate > currentDate.AddDays(-60) && p.DueDate <= currentDate.AddDays(-31)) ? p.Balance : 0,
                                BalanceFrom61To90Days = (p.DueDate >= currentDate.AddDays(-90) && p.DueDate < currentDate.AddDays(-61)) ? p.Balance : 0,
                                BalanceFrom91To120Days = (p.DueDate >= currentDate.AddDays(-120) && p.DueDate < currentDate.AddDays(-91)) ? p.Balance : 0,
                                BalanceMoreThan120Days = (p.DueDate < currentDate.AddDays(-121)) ? p.Balance : 0,
                                DaysExpired = ((p.DueDate.Date - currentDate.Date).Days) * -1 < 0 ? 0 : ((p.DueDate.Date - currentDate.Date).Days) * -1
                            }).OrderByDescending(x=> x.DaysExpired).ToList();
            return response;
        }
        public void AddPaymentSaleCounted(PaymentSale request)
        {
            request.IsValid();
            try
            {
               // request.DocDate = DateTime.Now;
                request.Complete = true;
                request.Canceled = false;
                _context.PaymentSale.Add(request);
                _context.SaveChanges();
                //Disminuimos el saldo por cobrar
                var bpjournal = new BPJornal
                {
                    BpId = request.CustomerId,
                    DocId = request.DocId,
                    BpType = "C",
                    TransValue = request.DocTotal * -1,
                    Documents = "Pago de factura de Venta",
                    DocumentReferent = request.DocId,
                    CreateBy = request.CreateBy,
                    CreateDate = DateTime.Now,
                    UUID = request.Uuid+"Contado",
                };
                _bPJornalServices.AddLineBPJournal(bpjournal);
                _customerServices.UpdateBalanceCustomer(request.CustomerId, request.DocTotal * -1);
                //Cerramos las facturas
                request.Detail.ForEach(x => CompleteInvoiceSale(x.InvoiceId, x.SumApplied));              
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
    }
}
