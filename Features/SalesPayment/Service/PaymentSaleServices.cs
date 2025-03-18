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
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Threading.Tasks;


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
        private List<PaymentSaleDto> GetBase(Expression<Func<PaymentSale, bool>> condition)
        {
            var payment = _context.PaymentSale
                .Include(x => x.Detail)
                    .ThenInclude(x => x.InvoiceSale)
                .Include(x => x.User)
                .Include(x => x.PayCondition)
                .Where(condition)  // Ahora sí se traducirá a SQL
                .AsNoTracking()
                .ToList();

            // Realizamos la transformación a DTO en memoria
            var result = payment.Select(e => new PaymentSaleDto
            {
                DocId = e.DocId,
                CustomerId = e.CustomerId,
                CustomerCode = e.CustomerCode,
                CustomerName = e.CustomerName,
                PayConditionId = e.PayConditionId,
                PayConditionName = e.PayCondition.PayConditionName,
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
                CreateByName = e.User.Name,
                Complete = e.Complete,
                Detail = e.Detail.Select(d => new PaymentSaleDetailDto
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
                    Balance = d.InvoiceSale.Balance + d.SumApplied
                }).ToList(),
                Uuid = e.Uuid,
                Offline = e.Offline
            }).ToList();

            return result.OrderByDescending(x => x.DocDate).ToList();
        }

        public List<PaymentSaleDto> GetPaymentSaleByDate(DateTime From, DateTime To)
        {
            return GetBase(x => x.DocDate.Date >= From && x.DocDate.Date <= To.Date);
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
                // Obtén la hora actual del servidor (Oregón)
                DateTime serverTime = DateTime.Now;
                // Define la zona horaria de Honduras
                TimeZoneInfo hondurasTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central America Standard Time");
                DateTime fechaPorDefecto = DateTime.MinValue;
                request.DocDate = request.DocDate != fechaPorDefecto ? request.DocDate : TimeZoneInfo.ConvertTime(serverTime, hondurasTimeZone);
                if (request.Offline)
                {
                    request.DocDate = TimeZoneInfo.ConvertTime(serverTime, hondurasTimeZone);

                    foreach (var detail in request.Detail)
                    {
                        var invoice = _context.InvoiceSale.FirstOrDefault(c => c.Uuid == detail.InvoiceReference);
                        if (invoice != null)
                        {
                            detail.InvoiceId = invoice.DocId;
                        }
                        else
                        {
                            throw new Exception($"No se encontró la factura, favor sincronizar primero la factura.");
                        }
                    }
                }
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
                    CreateDate = request.DocDate,
                    UUID = request.Uuid,
                };
                _bPJornalServices.AddLineBPJournal(bpjournal);
                _customerService.UpdateBalanceCustomer(request.CustomerId, request.DocTotal * -1);
                //Cerramos las facturas
                request.Detail.ForEach(x => _salesServices.CompleteInvoiceSaleFromPayment(x.InvoiceId, x.SumApplied, request.Offline, x.InvoiceReference));
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
                    CreateDate = DateTime.Now,
                    UUID = request.Uuid,
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
        public async Task<List<PaymentSaleDto>> CanceledPaymentSales(int docId)
        {
            // Cargar el pago y sus detalles en una sola consulta de manera asíncrona
            var currentPayment = await _context.PaymentSale
                .Include(x => x.Detail)
                .FirstOrDefaultAsync(x => x.DocId == docId);

            if (currentPayment == null)
                throw new Exception("No existe este pago, comuníquese con el administrador del sistema.");

            // Verificar liquidación usando una consulta más eficiente y asíncrona
            var hasActiveLiquidation = await _context.LiquidationDetail
                .Where(x => x.DocNum == docId && x.DocType.Contains("Pago"))
                .Join(_context.Liquidation,
                    detail => detail.LiquidationId,
                    liquidation => liquidation.IdLiquidation,
                    (detail, liquidation) => liquidation.Active)
                .AnyAsync(x => x);

            if (hasActiveLiquidation)
                throw new Exception("No puede cancelar este pago, porque ya esta liquidado. Anule primero la liquidacion.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Actualizar el pago
                currentPayment.Canceled = true;

                // Crear registro en BPJournal
                var bpjournal = new BPJornal
                {
                    BpId = currentPayment.CustomerId,
                    DocId = currentPayment.DocId,
                    BpType = "C",
                    TransValue = currentPayment.DocTotal,
                    Documents = "Pago de factura de Venta - Anulación",
                    DocumentReferent = currentPayment.DocId,
                    CreateBy = currentPayment.CreateBy,
                    CreateDate = DateTime.Now,
                    UUID = $"{currentPayment.Uuid}AnulaciónPago",
                };

                // Realizar las operaciones secuencialmente para evitar conflictos de contexto
                await _bPJornalServices.AddLineBPJournalAsync(bpjournal);
                await _customerService.UpdateBalanceCustomerAsync(currentPayment.CustomerId, currentPayment.DocTotal);

                // Actualizar las facturas secuencialmente
                foreach (var detail in currentPayment.Detail)
                {
                    await _salesServices.CompleteInvoiceSaleAsync(detail.InvoiceId, detail.SumApplied * -1);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }

            return GetPaymentSaleById(docId);
        }

        public List<PaymentSaleDto> EditPaymentSale(PaymentSale request)
        {
            var currentInvoice = _context.PaymentSale.Where(x => x.DocId == request.DocId).FirstOrDefault();
            if (currentInvoice == null) throw new Exception("Error: No existe esta orden, comuníquese con el administrador del sistema.");
            _context.Database.BeginTransaction();
            try
            {
                currentInvoice.Reference = request.Reference;
                currentInvoice.Comment = request.Comment;
                _context.PaymentSale.Update(currentInvoice);
                _context.SaveChanges();       
                _context.Database.CommitTransaction();
            }
            catch (Exception ex)
            {
                _context.Database.RollbackTransaction();
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
            return GetPaymentSaleById(request.DocId);
        }



    }
}
