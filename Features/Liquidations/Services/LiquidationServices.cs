using Org.BouncyCastle.Asn1.Ocsp;
using Pos.WebApi.Features.Expenses.Dto;
using Pos.WebApi.Features.Expenses.Entities;
using Pos.WebApi.Features.Liquidations.Dto;
using Pos.WebApi.Features.Liquidations.Entities;
using Pos.WebApi.Infraestructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pos.WebApi.Features.Liquidations.Services
{
    public class LiquidationServices
    {
        private readonly PosDbContext _context;

        public LiquidationServices(PosDbContext posDbContext)
        {
            _context = posDbContext;
        }

        public List<LiquidationDto> GetLiquidationBase(Func<Liquidation, bool> condition)
        {
            var liquidation = _context.Liquidation.Where(condition).ToList();
            var result = (from e in liquidation
                          join user in _context.User on e.CreatedBy equals user.UserId
                          join seller in _context.Seller on e.SellerId equals seller.SellerId
                          select new LiquidationDto
                          {
                              IdLiquidation = e.IdLiquidation,
                              SellerId = e.SellerId,
                              SellerName = seller.SellerName,
                              From = e.From,
                              To = e.To,
                              SaleCredit = e.SaleCredit,
                              SaleCash = e.SaleCash,
                              SaleTotal = e.SaleTotal,
                              PaidTotal = e.PaidTotal,
                              ExpenseTotal = e.ExpenseTotal,
                              Total = e.Total,
                              Deposit = e.Deposit,
                              TotalDifference = e.TotalDifference,
                              CreatedDate = e.CreatedDate,
                              CreatedByName = user.Name,
                              CreatedBy = e.CreatedBy,
                              Active = e.Active,
                              Detail = (from d in _context.LiquidationDetail                                       
                                        where d.LiquidationId == e.IdLiquidation
                                        select new LiquidationDetail
                                        {
                                            LiquidationDetailId = d.LiquidationDetailId,
                                            DocNum = d.DocNum,
                                            DocType = d.DocType,
                                            Reference = d.Reference,
                                            DocDate = d.DocDate,
                                            CustomerCode = d.CustomerCode,
                                            CustomerName = d.CustomerName,
                                            DocTotal = d.DocTotal,
                                            LiquidationId = d.LiquidationId
                                        }).ToList(),

                          }).ToList();
            return result;
        }

        public List<LiquidationDto> GetLiquidationByDate(DateTime From, DateTime To)
        {
            return GetLiquidationBase(x => x.CreatedDate.Date >= From.Date && x.CreatedDate.Date <= To.Date).ToList();
        }
        public List<LiquidationView> GetLiquidationsBySellerAndDate(DateTime From, DateTime To, int sellerId)
        {
            var result = _context.LiquidationView.Where(x => x.DocDate.Date >= From.Date && x.DocDate.Date <= To.Date && x.SellerId == sellerId).ToList();
            return result;
        }

        public List<LiquidationDto> AddLiquidation(Liquidation request)
        {
            request.IsValid();
            _context.Database.BeginTransaction();
            try
            {
                request.Detail.ForEach(x => x.LiquidationDetailId = 0);
                request.CreatedDate = DateTime.Now;
                request.Active = true;
                _context.Liquidation.Add(request);
                _context.SaveChanges();
                _context.Database.CommitTransaction();
            }
            catch (Exception ex)
            {
                _context.Database.RollbackTransaction();
                throw new Exception(ex.Message);
            }
            return GetLiquidationByDate(request.CreatedDate, request.CreatedDate);
        }

        public List<LiquidationDto> EditLiquidation(Liquidation request)
        {
            request.IsValid();
            var currentInvoice = _context.Liquidation.Where(x => x.IdLiquidation == request.IdLiquidation).FirstOrDefault();
            if (currentInvoice == null) throw new Exception("No existe este gasto, comuniquese con el administrador del sistema.");     
            _context.Database.BeginTransaction();
            try
            {
                currentInvoice.Deposit = request.Deposit;
                currentInvoice.SaleCash = request.SaleCash;
                currentInvoice.SaleTotal = request.SaleTotal;
                currentInvoice.SaleCredit = request.SaleCredit;
                currentInvoice.PaidTotal = request.PaidTotal;
                currentInvoice.Total = request.Total;
                currentInvoice.ExpenseTotal = request.ExpenseTotal;
                currentInvoice.TotalDifference = request.TotalDifference;
                currentInvoice.CreatedDate = DateTime.Now;
                currentInvoice.Active = true;
                _context.Liquidation.Update(currentInvoice);
                _context.SaveChanges();
                _context.Database.CommitTransaction();
            }
            catch (Exception ex)
            {
                _context.Database.RollbackTransaction();
                throw new Exception(ex.Message);
            }
            return GetLiquidationByDate(request.CreatedDate, request.CreatedDate);
        }

        public List<LiquidationDto> CancelLiquidation(Liquidation request)
        {
            request.IsValid();
            var currentInvoice = _context.Liquidation.Where(x => x.IdLiquidation == request.IdLiquidation).FirstOrDefault();
            if (currentInvoice == null) throw new Exception("No existe este gasto, comuniquese con el administrador del sistema.");
            _context.Database.BeginTransaction();
            try
            {
                currentInvoice.Active = false;
                _context.Liquidation.Update(currentInvoice);
                _context.SaveChanges();
                _context.Database.CommitTransaction();
            }
            catch (Exception ex)
            {
                _context.Database.RollbackTransaction();
                throw new Exception(ex.Message);
            }
            return GetLiquidationByDate(request.CreatedDate, request.CreatedDate);
        }

    }
}
