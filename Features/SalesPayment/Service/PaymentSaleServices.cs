using Pos.WebApi.Features.Common.Entities;
using Pos.WebApi.Features.Common;
using Pos.WebApi.Features.Customers.Services;
using Pos.WebApi.Features.Sales.Services;
using Pos.WebApi.Features.SalesPayment.Dto;
using Pos.WebApi.Infraestructure;
using System.Collections.Generic;
using System;
using Pos.WebApi.Features.SalesPayment.Entities;
using System.Linq;
using Pos.WebApi.Features.Purchase.Services;
using Org.BouncyCastle.Asn1.Ocsp;

namespace Pos.WebApi.Features.SalesPayment.Service
{
    public class PaymentSaleServices
    {
        private readonly PosDbContext _context;
        private readonly CustomerServices _customerService;
        private readonly BPJornalServices _bPJornalServices;
        private readonly SalesServices _salesServices;

        public PaymentSaleServices(PosDbContext posDbContext, CustomerServices priceService, BPJornalServices bPJornalServices, SalesServices salesServices)
        {
            _context = posDbContext;
            _customerService = priceService;
            _bPJornalServices = bPJornalServices;
            _salesServices = salesServices;
        }
        public List<PaymentSaleDto> GetPaymentSaleById(int id)
        {
            var result = GetBase(x => x.DocId == id).ToList();
            return result;
        }
        public List<PaymentSaleDto> GetPaymentSaleActive()
        {
            var result = GetBase(x => x.Complete == false).ToList();
            return result;
        }
        public List<PaymentSaleDto> GetPaymentSaleByDate(DateTime From, DateTime To)
        {
            var result = GetBase(x => x.DocDate.Date >= From && x.DocDate.Date <= To.Date).ToList();
            return result;
        }
        private List<PaymentSaleDto> GetBase(Func<PaymentSale, bool> condition)
        {
            var payment = _context.PaymentSale.Where(condition).ToList();
            var userId = payment.Select(x => x.CreateBy).Distinct().ToList();
            var user = _context.User.Where(x => userId.Contains(x.UserId)).ToList();
            var paymentId = payment.Select(x => x.DocId).Distinct().ToList();
            var paymentDetail = _context.PaymentSaleDetail.Where(x => paymentId.Contains(x.DocId)).ToList();
            var payId = payment.Select(x => x.PayConditionId).Distinct().ToList();
            var pays = _context.PayCondition.Where(x => payId.Contains(x.PayConditionId)).ToList();

            var edetail = (from d in paymentDetail
                           select new PaymentSaleDetailDto
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
                               InvoiceDate = d.InvoiceDate,
                               SumApplied = d.SumApplied,
                           }).ToList();

            var result = (from e in payment
                          join d in edetail on e.DocId equals d.DocId into detail
                          join u in user on e.CreateBy equals u.UserId
                          join p in pays on e.PayConditionId equals p.PayConditionId
                          select new PaymentSaleDto
                          {
                              DocId = e.DocId,
                              CustomerId = e.CustomerId,
                              CustomerCode = e.CustomerCode,
                              CustomerName = e.CustomerName,
                              PayConditionId = e.PayConditionId,
                              PayConditionName = p.PayConditionName,
                              DocDate = e.DocDate,
                              SellerId = e.SellerId,
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
        public List<PaymentSaleDto> GetPaymentSale()
        {
            var payment = _context.PaymentSale.ToList();
            var userId = payment.Select(x => x.CreateBy).Distinct().ToList();
            var user = _context.User.Where(x => userId.Contains(x.UserId)).ToList();
            var paymentId = payment.Select(x => x.DocId).Distinct().ToList();
            var paymentDetail = _context.PaymentSaleDetail.Where(x => paymentId.Contains(x.DocId)).ToList();
            var payId = payment.Select(x => x.PayConditionId).Distinct().ToList();
            var pays = _context.PayCondition.Where(x => payId.Contains(x.PayConditionId)).ToList();

            var edetail = (from d in paymentDetail
                           select new PaymentSaleDetailDto
                           {
                               DocDetailId = d.DocDetailId,
                               DocId = d.DocId,
                               InvoiceId = d.InvoiceId,
                               InvoiceReference = d.InvoiceReference,
                               DueDate = d.DueDate,
                               LineTotal = d.LineTotal,
                               InvoiceDate = d.InvoiceDate,
                               SumApplied = d.SumApplied,
                           }).ToList();

            var result = (from e in payment
                          join d in edetail on e.DocId equals d.DocId into detail
                          join u in user on e.CreateBy equals u.UserId
                          join p in pays on e.PayConditionId equals p.PayConditionId
                          select new PaymentSaleDto
                          {
                              DocId = e.DocId,
                              CustomerId = e.CustomerId,
                              CustomerCode = e.CustomerCode,
                              CustomerName = e.CustomerName,
                              PayConditionId = e.PayConditionId,
                              PayConditionName = p.PayConditionName,
                              SellerId = e.SellerId,
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
        public List<PaymentSaleDto> AddPaymentSale(PaymentSale request)
        {
            request.IsValid();
            _context.Database.BeginTransaction();
            try
            {
                request.DocDate = DateTime.Now;
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
                    CreateDate = DateTime.Now
                };
                _bPJornalServices.AddLineBPJournal(bpjournal);
                _customerService.UpdateBalanceCustomer(request.CustomerId, request.DocTotal * -1);
                //Cerramos las facturas
                request.Detail.ForEach(x => _salesServices.CompleteInvoiceSale(x.InvoiceId, x.SumApplied));
                _context.Database.CommitTransaction();
            }
            catch (Exception ex)
            {
                _context.Database.RollbackTransaction();
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
            return GetPaymentSaleById(request.DocId);
        }
        public string AddPaymentSaleCounted(PaymentSale request)
        {
            request.IsValid();
            try
            {
                request.DocDate = DateTime.Now;
                request.DocTotal = request.Detail.Sum(x => x.LineTotal);
                request.Complete = true;
                request.Canceled = false;
                _context.PaymentSale.Add(request);
                _context.SaveChanges();
                //Disminuimos el saldo por pagar
                var bpjournal = new BPJornal
                {
                    BpId = request.CustomerId,
                    DocId = request.DocId,
                    BpType = "C",
                    TransValue = request.DocTotal * -1,
                    Documents = "Pago de factura de Venta",
                    DocumentReferent = request.DocId,
                    CreateBy = request.CreateBy,
                    CreateDate = DateTime.Now
                };
                _bPJornalServices.AddLineBPJournal(bpjournal);
                _customerService.UpdateBalanceCustomer(request.CustomerId, request.DocTotal * -1);
                //Cerramos las facturas
                request.Detail.ForEach(x => _salesServices.CompleteInvoiceSale(x.InvoiceId, x.SumApplied)); 
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return "OK";
        }
        public List<PaymentSaleDto> CanceledPaymentSales(int docId)
        {
            var currentPayment = _context.PaymentSale.Where(x => x.DocId == docId).FirstOrDefault();
            if (currentPayment == null) throw new Exception("No existe este pago, comuniquese con el administrador del sistema.");
            var liquidation = _context.LiquidationDetail.Where(x => x.DocNum == docId && x.DocType.Contains("Pago")).FirstOrDefault();
            if (liquidation != null)
            {
                var liquitationStatus = _context.Liquidation.Where(x => x.IdLiquidation == liquidation.LiquidationId).FirstOrDefault();
                if (liquitationStatus.Active == true) throw new Exception("No puede cancelar este pago, porque ya esta liquidado. Anule primero la liquidacion.");
            }
            _context.Database.BeginTransaction();
            try
            {
                currentPayment.Canceled = true;
                //Aumentamos el saldo por cobrar
                var bpjournal = new BPJornal
                {
                    BpId = currentPayment.CustomerId,
                    DocId = currentPayment.DocId,
                    BpType = "C",
                    TransValue = currentPayment.DocTotal,
                    Documents = "Pago de factura de Venta - Anulacion",
                    DocumentReferent = currentPayment.DocId,
                    CreateBy = currentPayment.CreateBy,
                    CreateDate = DateTime.Now
                };
                _bPJornalServices.AddLineBPJournal(bpjournal);
                _customerService.UpdateBalanceCustomer(currentPayment.CustomerId, currentPayment.DocTotal);
                _context.PaymentSale.Update(currentPayment);
                var detail = _context.PaymentSaleDetail.Where(x => x.DocId == docId).ToList();

                detail.ForEach(x => _salesServices.CompleteInvoiceSale(x.InvoiceId, x.SumApplied*-1));
                _context.SaveChanges();
                _context.Database.CommitTransaction();
            }
            catch (Exception ex)
            {
                _context.Database.RollbackTransaction();
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
            return GetPaymentSaleById(docId);
        }



    }
}
