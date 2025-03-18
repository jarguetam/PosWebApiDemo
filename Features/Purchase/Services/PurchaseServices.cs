using Pos.WebApi.Features.InventoryTransactions.Services;
using Pos.WebApi.Features.Items.Services;
using Pos.WebApi.Features.Purchase.Dto;
using Pos.WebApi.Features.Purchase.Entities;
using Pos.WebApi.Infraestructure;
using System.Collections.Generic;
using System.Linq;
using System;
using Pos.WebApi.Features.InventoryTransactions.Entities;
using Pos.WebApi.Features.Items.Entities;
using Pos.WebApi.Features.Customers.Services;
using Pos.WebApi.Features.Common;
using Pos.WebApi.Features.Common.Entities;
using Pos.WebApi.Features.Suppliers.Services;
using Pos.WebApi.Features.PurchasePayment.Entitie;
using Pos.WebApi.Features.SalesPayment.Entities;


namespace Pos.WebApi.Features.Purchase.Services
{
    public class PurchaseServices
    {
        private readonly PosDbContext _context;
        private readonly ItemJournalServices _journalServices;
        private readonly WareHouseServices _wareHouseServices;
        private readonly ItemServices _itemServices;
        private readonly CustomerServices _priceService;
        private readonly BPJornalServices _bPJornalServices;
        private readonly SupplierServices _supplierServices;
        public PurchaseServices(PosDbContext posDbContext, BPJornalServices bPJornalServices, ItemJournalServices journalServices, WareHouseServices wareHouseServices, ItemServices itemServices, CustomerServices priceService, SupplierServices supplierServices)
        {
            _context = posDbContext;
            _journalServices = journalServices;
            _wareHouseServices = wareHouseServices;
            _itemServices = itemServices;
            _priceService = priceService;
            _bPJornalServices = bPJornalServices;
            _supplierServices = supplierServices;
        }

        public List<OrderPurchaseDto> GetOrderPurchaseById(int id)
        {
            var result = GetBase(x => x.DocId == id).ToList();
            return result;
        }
        public List<OrderPurchaseDto> GetOrderPurchaseActive()
        {
            var result = GetBase(x => x.Complete == false).ToList();
            return result;
        }
        public List<OrderPurchaseDto> GetOrderPurchaseByDate(DateTime From, DateTime To)
        {
            var result = GetBase(x => x.DocDate.Date >= From && x.DocDate.Date <= To.Date).ToList();
            return result;
        }
        private List<OrderPurchaseDto> GetBase(Func<OrderPurchase, bool> condition)
        {
            var order = _context.OrderPurchase.Where(condition).ToList();
            var userId = order.Select(x => x.CreateBy).Distinct().ToList();
            var user = _context.User.Where(x => userId.Contains(x.UserId)).ToList();
            var whsCode = order.Select(x => x.WhsCode).Distinct().ToList();
            var whs = _context.WareHouse.Where(x => whsCode.Contains(x.WhsCode)).ToList();
            var orderId = order.Select(x => x.DocId).Distinct().ToList();
            var orderDetail = _context.OrderPurchaseDetail.Where(x => orderId.Contains(x.DocId)).ToList();
            var itemId = orderDetail.Select(x => x.ItemId).Distinct().ToList();
            var items = _context.Item.Where(x => itemId.Contains(x.ItemId)).ToList();
            var unit = orderDetail.Select(x => x.UnitOfMeasureId).Distinct().ToList();
            var units = _context.UnitOfMeasure.Where(x => unit.Contains(x.UnitOfMeasureId)).ToList();
            var payId = order.Select(x => x.PayConditionId).Distinct().ToList();
            var pays = _context.PayCondition.Where(x => payId.Contains(x.PayConditionId)).ToList();

            var edetail = (from d in orderDetail
                           join i in items on d.ItemId equals i.ItemId
                           join w in whs on d.WhsCode equals w.WhsCode
                           join u in units on d.UnitOfMeasureId equals u.UnitOfMeasureId
                           where d.IsDelete == false
                           select new OrderPurchaseDetailDto
                           {
                               DocDetailId = d.DocDetailId,
                               DocId = d.DocId,
                               ItemId = i.ItemId,
                               ItemCode = i.ItemCode,
                               ItemName = i.ItemName,
                               Quantity = d.Quantity,
                               DueDate = d.DueDate,
                               Price = d.Price,
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
                          select new OrderPurchaseDto
                          {
                              DocId = e.DocId,
                              SupplierId = e.SupplierId,
                              SupplierCode = e.SupplierCode,
                              SupplierName = e.SupplierName,
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
                              taxSupplier = e.Tax > 0 ? true : false
                          }).ToList();
            return result.OrderByDescending(x => x.DocDate).ToList();
        }
        public List<OrderPurchaseDto> GetOrderPurchase()
        {
            var order = _context.OrderPurchase.ToList();
            var userId = order.Select(x => x.CreateBy).Distinct().ToList();
            var user = _context.User.Where(x => userId.Contains(x.UserId)).ToList();
            var whsCode = order.Select(x => x.WhsCode).Distinct().ToList();
            var whs = _context.WareHouse.Where(x => whsCode.Contains(x.WhsCode)).ToList();
            var orderId = order.Select(x => x.DocId).Distinct().ToList();
            var orderDetail = _context.OrderPurchaseDetail.Where(x => orderId.Contains(x.DocId)).ToList();
            var itemId = orderDetail.Select(x => x.ItemId).Distinct().ToList();
            var items = _context.Item.Where(x => itemId.Contains(x.ItemId)).ToList();
            var unit = orderDetail.Select(x => x.UnitOfMeasureId).Distinct().ToList();
            var units = _context.UnitOfMeasure.Where(x => unit.Contains(x.UnitOfMeasureId)).ToList();
            var payId = order.Select(x => x.PayConditionId).Distinct().ToList();
            var pays = _context.PayCondition.Where(x => payId.Contains(x.PayConditionId)).ToList();

            var edetail = (from d in orderDetail
                           join i in items on d.ItemId equals i.ItemId
                           join w in whs on d.WhsCode equals w.WhsCode
                           join u in units on d.UnitOfMeasureId equals u.UnitOfMeasureId
                           where d.IsDelete == false
                           select new OrderPurchaseDetailDto
                           {
                               DocDetailId = d.DocDetailId,
                               DocId = d.DocId,
                               ItemId = i.ItemId,
                               ItemCode = i.ItemCode,
                               ItemName = i.ItemName,
                               Quantity = d.Quantity,
                               DueDate = d.DueDate,
                               Price = d.Price,
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
                          select new OrderPurchaseDto
                          {
                              DocId = e.DocId,
                              SupplierId = e.SupplierId,
                              SupplierCode = e.SupplierCode,
                              SupplierName = e.SupplierName,
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
                              taxSupplier = e.Tax > 0 ? true : false
                          }).ToList();
            return result.OrderByDescending(x => x.DocDate).ToList();
        }
        public List<OrderPurchaseDto> AddOrderPurchase(OrderPurchase request)
        {
            request.IsValid();
            _context.Database.BeginTransaction();
            try
            {
                DateTime fechaPorDefecto = DateTime.MinValue;
                request.DocDate = request.DocDate != fechaPorDefecto ? request.DocDate : DateTime.Now;
                request.DocQty = request.Detail.Sum(x => x.Quantity);
                request.Complete = false;
                request.Canceled = false;
                _context.OrderPurchase.Add(request);
                _context.SaveChanges();
                _context.Database.CommitTransaction();
            }
            catch (Exception ex)
            {
                _context.Database.RollbackTransaction();
                throw new Exception(ex.Message);
            }
            return GetOrderPurchaseById(request.DocId);
        }

        public List<OrderPurchaseDto> EditOrderPurchase(OrderPurchase request)
        {
            request.IsValid();
            var currentOrder = _context.OrderPurchase.Where(x => x.DocId == request.DocId).FirstOrDefault();
            if (currentOrder == null) throw new Exception("No existe esta orden, comuniquese con el administrador del sistema.");
            _context.Database.BeginTransaction();
            try
            {
                currentOrder.SupplierCode = request.SupplierCode;
                currentOrder.SupplierName = request.SupplierName;
                currentOrder.SupplierId = request.SupplierId;
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
                var currentDetail = _context.OrderPurchaseDetail.Where(x => x.DocId == currentOrder.DocId).ToList();
                // var removeLine = request.Detail.Where(x => x.IsDelete == true).ToList();
                ////Borramos las lineas
                //_context.OrderPurchaseDetail.RemoveRange(removeLine);
                var newLine = request.Detail.Where(x => x.DocDetailId == 0).ToList();
                //Agregamos las nuevas lineas
                _context.OrderPurchaseDetail.AddRange(newLine);
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
                        item.WhsCode = currentOrder.WhsCode;
                        item.UnitOfMeasureId = itemToUpdate.UnitOfMeasureId;
                        item.LineTotal = itemToUpdate.LineTotal;
                        item.IsDelete = itemToUpdate.IsDelete;
                    }

                });
                _context.OrderPurchaseDetail.UpdateRange(currentDetail);
                _context.OrderPurchase.Update(currentOrder);
                _context.SaveChanges();
                _context.Database.CommitTransaction();
            }
            catch (Exception ex)
            {
                _context.Database.RollbackTransaction();
                throw new Exception(ex.Message);
            }
            return GetOrderPurchaseById(request.DocId);
        }

        public List<OrderPurchaseDto> CanceledOrderPurchase(int docId)
        {
            var currentOrder = _context.OrderPurchase.Where(x => x.DocId == docId).FirstOrDefault();
            if (currentOrder == null) throw new Exception("No existe esta orden, comuniquese con el administrador del sistema.");
            _context.Database.BeginTransaction();
            try
            {
                currentOrder.Canceled = true;
                currentOrder.Complete = true;
                _context.OrderPurchase.Update(currentOrder);
                _context.SaveChanges();
                _context.Database.CommitTransaction();
            }
            catch (Exception ex)
            {
                _context.Database.RollbackTransaction();
                throw new Exception(ex.Message);
            }
            return GetOrderPurchaseById(docId);
        }

        public List<OrderPurchaseDto> CompleteOrderPurchase(int docId, bool status)
        {
            var currentOrder = _context.OrderPurchase.Where(x => x.DocId == docId).FirstOrDefault();
            if (currentOrder == null) throw new Exception("No existe esta orden, comuniquese con el administrador del sistema.");
            try
            {
                currentOrder.Complete = status;
                _context.OrderPurchase.Update(currentOrder);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
            return GetOrderPurchaseById(docId);
        }

        //Invoice
        public List<InvoicePurchaseDto> GetInvoicePurchaseById(int id)
        {
            var result = GetBaseInvoice(x => x.DocId == id).ToList();
            return result;
        }
        public List<InvoicePurchaseDto> GetInvoicePurchaseActive()
        {
            var result = GetBaseInvoice(x => x.Complete == false).ToList();
            return result;
        }

        public List<InvoicePurchaseDto> GetInvoicePurchaseActiveSupplier(int idSupplier)
        {
            var result = GetBaseInvoice(x => x.Complete == false && x.SupplierId == idSupplier).ToList();
            return result;
        }
        public List<InvoicePurchaseDto> GetInvoicePurchaseByDate(DateTime From, DateTime To)
        {
            var result = GetBaseInvoice(x => x.DocDate.Date >= From && x.DocDate.Date <= To.Date).ToList();
            return result;
        }
        private List<InvoicePurchaseDto> GetBaseInvoice(Func<InvoicePurchase, bool> condition)
        {
            var invoice = _context.InvoicePurchase.Where(condition).ToList();
            var userId = invoice.Select(x => x.CreateBy).Distinct().ToList();
            var user = _context.User.Where(x => userId.Contains(x.UserId)).ToList();
            var whsCode = invoice.Select(x => x.WhsCode).Distinct().ToList();
            var whs = _context.WareHouse.Where(x => whsCode.Contains(x.WhsCode)).ToList();
            var invoiceId = invoice.Select(x => x.DocId).Distinct().ToList();
            var invoiceDetail = _context.InvoicePurchaseDetail.Where(x => invoiceId.Contains(x.DocId)).ToList();
            var itemId = invoiceDetail.Select(x => x.ItemId).Distinct().ToList();
            var items = _context.Item.Where(x => itemId.Contains(x.ItemId)).ToList();
            var unit = invoiceDetail.Select(x => x.UnitOfMeasureId).Distinct().ToList();
            var units = _context.UnitOfMeasure.Where(x => unit.Contains(x.UnitOfMeasureId)).ToList();
            var payId = invoice.Select(x => x.PayConditionId).Distinct().ToList();
            var pays = _context.PayCondition.Where(x => payId.Contains(x.PayConditionId)).ToList();

            var edetail = (from d in invoiceDetail
                           join i in items on d.ItemId equals i.ItemId
                           join w in whs on d.WhsCode equals w.WhsCode
                           join u in units on d.UnitOfMeasureId equals u.UnitOfMeasureId
                           where d.IsDelete == false
                           select new InvoicePurchaseDetailDto
                           {
                               DocDetailId = d.DocDetailId,
                               DocId = d.DocId,
                               ItemId = i.ItemId,
                               ItemCode = i.ItemCode,
                               ItemName = i.ItemName,
                               Quantity = d.Quantity,
                               DueDate = d.DueDate,
                               Price = d.Price,
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
                          select new InvoicePurchaseDto
                          {
                              DocId = e.DocId,
                              SupplierId = e.SupplierId,
                              SupplierCode = e.SupplierCode,
                              SupplierName = e.SupplierName,
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
                              taxSupplier = e.Tax > 0 ? true : false,
                              Balance = e.Balance,
                              PaidToDate = e.PaidToDate
                          }).ToList();
            return result.OrderByDescending(x => x.DocDate).ToList();
        }
        public List<InvoicePurchaseDto> GetInvoicePurchase()
        {
            var invoice = _context.InvoicePurchase.ToList();
            var userId = invoice.Select(x => x.CreateBy).Distinct().ToList();
            var user = _context.User.Where(x => userId.Contains(x.UserId)).ToList();
            var whsCode = invoice.Select(x => x.WhsCode).Distinct().ToList();
            var whs = _context.WareHouse.Where(x => whsCode.Contains(x.WhsCode)).ToList();
            var invoiceId = invoice.Select(x => x.DocId).Distinct().ToList();
            var invoiceDetail = _context.InvoicePurchaseDetail.Where(x => invoiceId.Contains(x.DocId)).ToList();
            var itemId = invoiceDetail.Select(x => x.ItemId).Distinct().ToList();
            var items = _context.Item.Where(x => itemId.Contains(x.ItemId)).ToList();
            var unit = invoiceDetail.Select(x => x.UnitOfMeasureId).Distinct().ToList();
            var units = _context.UnitOfMeasure.Where(x => unit.Contains(x.UnitOfMeasureId)).ToList();
            var payId = invoice.Select(x => x.PayConditionId).Distinct().ToList();
            var pays = _context.PayCondition.Where(x => payId.Contains(x.PayConditionId)).ToList();

            var edetail = (from d in invoiceDetail
                           join i in items on d.ItemId equals i.ItemId
                           join w in whs on d.WhsCode equals w.WhsCode
                           join u in units on d.UnitOfMeasureId equals u.UnitOfMeasureId
                           where d.IsDelete == false
                           select new InvoicePurchaseDetailDto
                           {
                               DocDetailId = d.DocDetailId,
                               DocId = d.DocId,
                               ItemId = i.ItemId,
                               ItemCode = i.ItemCode,
                               ItemName = i.ItemName,
                               Quantity = d.Quantity,
                               DueDate = d.DueDate,
                               Price = d.Price,
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
                          select new InvoicePurchaseDto
                          {
                              DocId = e.DocId,
                              SupplierId = e.SupplierId,
                              SupplierCode = e.SupplierCode,
                              SupplierName = e.SupplierName,
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
                              taxSupplier = e.Tax > 0 ? true : false
                          }).ToList();
            return result.OrderByDescending(x => x.DocDate).ToList();
        }
        public List<InvoicePurchaseDto> AddInvoicePurchase(InvoicePurchase request)
        {
            request.IsValid();
            _context.Database.BeginTransaction();
            try
            {
                DateTime fechaPorDefecto = DateTime.MinValue;
                request.DocDate = request.DocDate != fechaPorDefecto ? request.DocDate : DateTime.Now;
                request.DocQty = request.Detail.Sum(x => x.Quantity);
                request.Complete = false;
                request.Canceled = false;
                request.Balance = request.DocTotal;
                request.Detail.ForEach(x => x.DocDetailId = 0);
                if (request.PayConditionId == 1)
                    request.Complete = true;
                _context.InvoicePurchase.Add(request);
                _context.SaveChanges();
                //Aumentamos inventario
                var journal = request.Detail.Select(x => new ItemJournal
                {
                    ItemId = x.ItemId,
                    WhsCode = x.WhsCode,
                    Quantity = x.Quantity,
                    Price = x.Price,
                    TransValue = x.Price * x.Quantity,
                    Documents = "Factura de Compra",
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
                    Stock = x.Quantity,
                    AvgPrice = x.Price,
                    CreateDate = DateTime.Now,
                    DueDate = x.DueDate
                }).ToList();
                warehouse.ForEach(x => _wareHouseServices.UpdateItemWareHouse(x));
                warehouse.ForEach(x => _itemServices.UpdateStockItem(x.ItemId));

                // warehouse.ForEach(x => _priceService.UpdatePriceListDetail(x.ItemId, x.AvgPrice));
                //Aumentamos el saldo por pagar
                var bpjournal = new BPJornal
                {
                    BpId = request.SupplierId,
                    DocId = request.DocId,
                    BpType = "P",
                    TransValue = request.DocTotal,
                    Documents = "Factura de Compra",
                    DocumentReferent = request.DocId,
                    CreateBy = request.CreateBy,
                    CreateDate = DateTime.Now,
                    UUID = Guid.NewGuid().ToString()
                };
                _bPJornalServices.AddLineBPJournal(bpjournal);
                _supplierServices.UpdateBalanceSupplier(request.SupplierId, request.DocTotal);
                //Si viene de un pedido, cerramos ese pedido
                if (request.DocReference != 0)
                    this.CompleteOrderPurchase(request.DocReference, true);
                //Si es de contado se aplica el pago
                if (request.PayConditionId == 1)
                {
                    var payment = new PaymentPurchase
                    {
                        SupplierId = request.SupplierId,
                        SupplierCode = request.SupplierCode,
                        SupplierName = request.SupplierName,
                        PayConditionId = request.PayConditionId,
                        Reference = request.DocId.ToString(),
                        DocDate = request.DocDate,
                        Comment = "Factura de contado: " + request.DocId,
                        DocTotal = request.DocTotal,
                        CreateBy = request.CreateBy,
                        CashSum = request.DocTotal,
                    };
                    var paymentDetail = new PaymentPurchaseDetail
                    {
                        InvoiceId = request.DocId,
                        InvoiceDate = request.DocDate,
                        InvoiceReference = request.DocId.ToString(),
                        DueDate = request.DueDate,
                        SubTotal = request.SubTotal,
                        TaxTotal = request.Tax,
                        DiscountTotal = request.DiscountsTotal,
                        LineTotal = request.DocTotal,
                        SumApplied = request.DocTotal
                    };
                    payment.Detail.Add(paymentDetail);
                    AddPaymentPurchaseCounted(payment);
                }
                _context.Database.CommitTransaction();
            }
            catch (Exception ex)
            {
                _context.Database.RollbackTransaction();
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
            return GetInvoicePurchaseById(request.DocId);
        }
        public List<InvoicePurchaseDto> EditInvoicePurchase(InvoicePurchase request)
        {
            request.IsValid();
            var currentInvoice = _context.InvoicePurchase.Where(x => x.DocId == request.DocId).FirstOrDefault();
            if (currentInvoice == null) throw new Exception("No existe esta orden, comuniquese con el administrador del sistema.");
            _context.Database.BeginTransaction();
            try
            {
                currentInvoice.DocDate = request.DocDate;
                currentInvoice.SupplierCode = request.SupplierCode;
                currentInvoice.SupplierName = request.SupplierName;
                currentInvoice.SupplierId = request.SupplierId;
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
                var currentDetail = _context.InvoicePurchaseDetail.Where(x => x.DocId == currentInvoice.DocId).ToList();
                var newLine = request.Detail.Where(x => x.DocDetailId == 0).ToList();
                //Agregamos las nuevas lineas
                _context.InvoicePurchaseDetail.AddRange(newLine);
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
                _context.InvoicePurchaseDetail.UpdateRange(currentDetail);
                _context.InvoicePurchase.Update(currentInvoice);
                _context.SaveChanges();
                _context.Database.CommitTransaction();
            }
            catch (Exception ex)
            {
                _context.Database.RollbackTransaction();
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
            return GetInvoicePurchaseById(request.DocId);
        }
        public List<InvoicePurchaseDto> CanceledInvoicePurchase(int docId)
        {
            var currentInvoice = _context.InvoicePurchase.Where(x => x.DocId == docId).FirstOrDefault();
            var currentDetail = _context.InvoicePurchaseDetail.Where(x => x.DocId == docId).ToList();
            if (currentInvoice == null) throw new Exception("No existe esta orden, comuniquese con el administrador del sistema.");
            _context.Database.BeginTransaction();
            try
            {
                currentInvoice.Canceled = true;
                currentInvoice.Complete = true;
                _context.InvoicePurchase.Update(currentInvoice);
                _context.SaveChanges();
                //Disminuimos inventario
                var journal = currentDetail.Select(x => new ItemJournal
                {
                    ItemId = x.ItemId,
                    WhsCode = x.WhsCode,
                    Quantity = x.Quantity * -1,
                    Price = x.Price * -1,
                    TransValue = (x.Price * x.Quantity) * -1,
                    Documents = "Factura de Compra -Anulacion",
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
                    Stock = x.Quantity * -1,
                    AvgPrice = x.Price,
                    CreateDate = DateTime.Now,
                    DueDate = x.DueDate
                }).ToList();
                warehouse.ForEach(x => _wareHouseServices.UpdateItemWareHouse(x));
                warehouse.ForEach(x => _itemServices.UpdateStockItem(x.ItemId));
                //  warehouse.ForEach(x => _priceService.UpdatePriceListDetail(x.ItemId, x.AvgPrice));
                //Aumentamos el saldo por pagar
                var bpjournal = new BPJornal
                {
                    BpId = currentInvoice.SupplierId,
                    DocId = currentInvoice.DocId,
                    BpType = "P",
                    TransValue = currentInvoice.DocTotal * -1,
                    Documents = "Factura de Compra - Anulacion",
                    DocumentReferent = currentInvoice.DocId,
                    CreateBy = currentInvoice.CreateBy,
                    CreateDate = DateTime.Now,
                    UUID = Guid.NewGuid().ToString()
                };
                _bPJornalServices.AddLineBPJournal(bpjournal);
                _supplierServices.UpdateBalanceSupplier(currentInvoice.SupplierId, currentInvoice.DocTotal * -1);
                //Si viene de un pedido, abrimos ese pedido
                if (currentInvoice.DocReference != 0)
                    this.CompleteOrderPurchase(currentInvoice.DocReference, false);
                _context.Database.CommitTransaction();

            }
            catch (Exception ex)
            {
                _context.Database.RollbackTransaction();
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
            return GetInvoicePurchaseById(docId);
        }
        public List<InvoicePurchaseDto> CompleteInvoicePurchase(int docId, decimal sumApplied)
        {
            var currentInvoice = _context.InvoicePurchase.Where(x => x.DocId == docId).FirstOrDefault();
            if (currentInvoice == null) throw new Exception("No existe esta orden, comuniquese con el administrador del sistema.");
            try
            {
                currentInvoice.PaidToDate = currentInvoice.PaidToDate + sumApplied;
                currentInvoice.Balance = currentInvoice.Balance - sumApplied;
                if (currentInvoice.Balance <= 0)
                    currentInvoice.Complete = true;
                else currentInvoice.Complete = false;
                _context.InvoicePurchase.Update(currentInvoice);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return GetInvoicePurchaseById(docId);
        }
        public List<SupplierAccountDto> GetAccountBalance()
        {
            var currentDate = DateTime.Now.Date;
            var purchase = _context.InvoicePurchase.Where(x => x.Balance > 0 && !x.Canceled).ToList();
            var supplierId = purchase.Select(x => x.SupplierId).Distinct().ToList();
            var suppliers = _context.Supplier.Where(x => supplierId.Contains(x.SupplierId)).ToList();
            var payId = suppliers.Select(x => x.PayConditionId).Distinct().ToList();
            var paycondition = _context.PayCondition.Where(x => payId.Contains(x.PayConditionId)).ToList();

            var response = (from p in purchase
                            join s in suppliers on p.SupplierId equals s.SupplierId
                            join pay in paycondition on p.PayConditionId equals pay.PayConditionId
                            orderby p.SupplierId
                            select new SupplierAccountDto
                            {
                                SupplierId = p.SupplierId,
                                SupplierCode = p.SupplierCode,
                                SupplierName = s.SupplierName,
                                PayConditionName = pay.PayConditionName,
                                InvoiceNumber = p.DocId,
                                Balance = s.Balance,
                                DueDate = p.DueDate,
                                UnexpiredBalance = p.DueDate > currentDate ? p.Balance : 0,
                                BalanceDue = p.DueDate < currentDate ? p.Balance : 0,
                                BalanceAt30Days = (p.DueDate >= currentDate.AddDays(-30) && p.DueDate <= currentDate) ? p.Balance : 0,
                                BalanceFrom31To60Days = (p.DueDate > currentDate.AddDays(-60) && p.DueDate <= currentDate.AddDays(-30)) ? p.Balance : 0,
                                BalanceFrom61To90Days = (p.DueDate >= currentDate.AddDays(-61) && p.DueDate < currentDate.AddDays(-90)) ? p.Balance : 0,
                                BalanceFrom91To120Days = (p.DueDate >= currentDate.AddDays(-91) && p.DueDate < currentDate.AddDays(-120)) ? p.Balance : 0,
                                BalanceMoreThan120Days = (p.DueDate < currentDate.AddDays(-121)) ? p.Balance : 0,
                                DaysExpired = ((p.DueDate.Date - currentDate.Date).Days) * -1 < 0 ? 0 : ((p.DueDate.Date - currentDate.Date).Days) * -1
                            }).ToList();
            return response;
        }

        public string AddPaymentPurchaseCounted(PaymentPurchase request)
        {
            request.IsValid();
            try
            {
                request.DocDate = DateTime.Now;
                request.DocTotal = request.Detail.Sum(x => x.LineTotal);
                request.Complete = true;
                request.Canceled = false;
                _context.PaymentPurchase.Add(request);
                _context.SaveChanges();
                //Disminuimos el saldo por pagar
                var bpjournal = new BPJornal
                {
                    BpId = request.SupplierId,
                    DocId = request.DocId,
                    BpType = "P",
                    TransValue = request.DocTotal * -1,
                    Documents = "Pago de factura de Compra",
                    DocumentReferent = request.DocId,
                    CreateBy = request.CreateBy,
                    CreateDate = DateTime.Now,
                    UUID = Guid.NewGuid().ToString()
                };
                //Cerramos las facturas
                request.Detail.ForEach(x => CompleteInvoicePurchase(x.InvoiceId, x.SumApplied));
                _bPJornalServices.AddLineBPJournal(bpjournal);
                _supplierServices.UpdateBalanceSupplier(request.SupplierId, request.DocTotal * -1);
                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return "OK";
        }
    }
}
