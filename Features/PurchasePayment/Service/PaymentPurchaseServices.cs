using Pos.WebApi.Features.Common;
using Pos.WebApi.Features.Customers.Services;
using Pos.WebApi.Features.PurchasePayment.Dto;
using Pos.WebApi.Features.PurchasePayment.Entitie;
using Pos.WebApi.Features.Suppliers.Services;
using Pos.WebApi.Infraestructure;
using System.Collections.Generic;
using System.Linq;
using System;
using Pos.WebApi.Features.Common.Entities;
using Pos.WebApi.Features.Purchase.Services;

namespace Pos.WebApi.Features.PurchasePayment.Service
{
    public class PaymentPurchaseServices
    {
        private readonly PosDbContext _context;
        private readonly CustomerServices _priceService;
        private readonly BPJornalServices _bPJornalServices;
        private readonly SupplierServices _supplierServices;
        private readonly PurchaseServices _purchaseServices;

        public PaymentPurchaseServices(PosDbContext posDbContext, CustomerServices priceService, BPJornalServices bPJornalServices, SupplierServices supplierServices, PurchaseServices purchaseServices)
        {
            _context = posDbContext;
            _priceService = priceService;
            _bPJornalServices = bPJornalServices;
            _supplierServices = supplierServices;
            _purchaseServices = purchaseServices;   
        }

        public List<PaymentPurchaseDto> GetPaymentPurchaseById(int id)
        {
            var result = GetBase(x => x.DocId == id).ToList();
            return result;
        }
        public List<PaymentPurchaseDto> GetPaymentPurchaseActive()
        {
            var result = GetBase(x => x.Complete == false).ToList();
            return result;
        }
        public List<PaymentPurchaseDto> GetPaymentPurchaseByDate(DateTime From, DateTime To)
        {
            var result = GetBase(x => x.DocDate.Date >= From && x.DocDate.Date <= To.Date).ToList();
            return result;
        }
        private List<PaymentPurchaseDto> GetBase(Func<PaymentPurchase, bool> condition)
        {
            var payment = _context.PaymentPurchase.Where(condition).ToList();
            var userId = payment.Select(x => x.CreateBy).Distinct().ToList();
            var user = _context.User.Where(x => userId.Contains(x.UserId)).ToList();       
            var paymentId = payment.Select(x => x.DocId).Distinct().ToList();
            var paymentDetail = _context.PaymentPurchaseDetail.Where(x => paymentId.Contains(x.DocId)).ToList();
            var payId = payment.Select(x => x.PayConditionId).Distinct().ToList();
            var pays = _context.PayCondition.Where(x => payId.Contains(x.PayConditionId)).ToList();

            var edetail = (from d in paymentDetail
                           select new PaymentPurchaseDetailDto
                           {
                               DocDetailId = d.DocDetailId,
                               DocId = d.DocId,
                               InvoiceId = d.InvoiceId,
                               InvoiceReference = d.InvoiceReference,
                               DueDate = d.DueDate,
                               SubTotal = d.SubTotal,
                               TaxTotal = d.TaxTotal,
                               DiscountTotal = d.DiscountTotal,
                               LineTotal = d.LineTotal,
                               InvoiceDate = d.InvoiceDate
                           }).ToList();

            var result = (from e in payment
                          join d in edetail on e.DocId equals d.DocId into detail
                          join u in user on e.CreateBy equals u.UserId
                          join p in pays on e.PayConditionId equals p.PayConditionId
                          select new PaymentPurchaseDto
                          {
                              DocId = e.DocId,
                              SupplierId = e.SupplierId,
                              SupplierCode = e.SupplierCode,
                              SupplierName = e.SupplierName,
                              PayConditionId = e.PayConditionId,
                              PayConditionName = p.PayConditionName,
                              DocDate = e.DocDate,
                              Canceled = e.Canceled,
                              Comment = e.Comment,
                              Reference = e.Reference,
                              CashSum = e.CashSum,
                              ChekSum = e.ChekSum,
                              TransferSum = e.TransferSum,
                              CardSum = e.CardSum,                        
                              DocTotal = e.DocTotal,
                              CreateBy = e.CreateBy,
                              CreateByName = u.Name,
                              Complete = e.Complete,
                              Detail = detail.ToList(),
                          }).ToList();
            return result.OrderByDescending(x => x.DocDate).ToList();
        }
        public List<PaymentPurchaseDto> GetPaymentPurchase()
        {
            var payment = _context.PaymentPurchase.ToList();
            var userId = payment.Select(x => x.CreateBy).Distinct().ToList();
            var user = _context.User.Where(x => userId.Contains(x.UserId)).ToList();
            var paymentId = payment.Select(x => x.DocId).Distinct().ToList();
            var paymentDetail = _context.PaymentPurchaseDetail.Where(x => paymentId.Contains(x.DocId)).ToList();
            var payId = payment.Select(x => x.PayConditionId).Distinct().ToList();
            var pays = _context.PayCondition.Where(x => payId.Contains(x.PayConditionId)).ToList();

            var edetail = (from d in paymentDetail
                           select new PaymentPurchaseDetailDto
                           {
                               DocDetailId = d.DocDetailId,
                               DocId = d.DocId,
                               InvoiceId = d.InvoiceId,
                               InvoiceReference = d.InvoiceReference,
                               DueDate = d.DueDate,
                               LineTotal = d.LineTotal,
                               InvoiceDate = d.InvoiceDate
                           }).ToList();

            var result = (from e in payment
                          join d in edetail on e.DocId equals d.DocId into detail
                          join u in user on e.CreateBy equals u.UserId
                          join p in pays on e.PayConditionId equals p.PayConditionId
                          select new PaymentPurchaseDto
                          {
                              DocId = e.DocId,
                              SupplierId = e.SupplierId,
                              SupplierCode = e.SupplierCode,
                              SupplierName = e.SupplierName,
                              PayConditionId = e.PayConditionId,
                              PayConditionName = p.PayConditionName,
                              DocDate = e.DocDate,
                              Canceled = e.Canceled,
                              Comment = e.Comment,
                              Reference = e.Reference,
                              CashSum = e.CashSum,
                              ChekSum = e.ChekSum,
                              TransferSum = e.TransferSum,
                              CardSum = e.CardSum,
                              DocTotal = e.DocTotal,
                              CreateBy = e.CreateBy,
                              CreateByName = u.Name,
                              Complete = e.Complete,
                              Detail = detail.ToList(),
                          }).ToList();
            return result.OrderByDescending(x => x.DocDate).ToList();
        }
        public List<PaymentPurchaseDto> AddPaymentPurchase(PaymentPurchase request)
        {
            request.IsValid();
            _context.Database.BeginTransaction();
            try
            {
                request.DocDate = DateTime.Now;
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
                    TransValue = request.DocTotal *-1,
                    Documents = "Pago de factura de Compra",
                    DocumentReferent = request.DocId,
                    CreateBy = request.CreateBy,
                    CreateDate = DateTime.Now
                };
                _bPJornalServices.AddLineBPJournal(bpjournal);
                _supplierServices.UpdateBalanceSupplier(request.SupplierId, request.DocTotal*-1);
                //Actualizamos las facturas
                request.Detail.ForEach(x => _purchaseServices.CompleteInvoicePurchase(x.InvoiceId, x.SumApplied));
                _context.Database.CommitTransaction();
            }
            catch (Exception ex)
            {
                _context.Database.RollbackTransaction();
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
            return GetPaymentPurchaseById(request.DocId);
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
                    CreateDate = DateTime.Now
                };
                _bPJornalServices.AddLineBPJournal(bpjournal);
                _supplierServices.UpdateBalanceSupplier(request.SupplierId, request.DocTotal * -1);
                //Cerramos las facturas
                request.Detail.Select(x => _purchaseServices.CompleteInvoicePurchase(x.InvoiceId, x.SumApplied));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return "OK";
        }
    }
}
