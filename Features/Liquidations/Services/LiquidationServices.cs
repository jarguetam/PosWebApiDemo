using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Asn1.Ocsp;
using Pos.WebApi.Features.Expenses.Dto;
using Pos.WebApi.Features.Expenses.Entities;
using Pos.WebApi.Features.Liquidations.Dto;
using Pos.WebApi.Features.Liquidations.Entities;
using Pos.WebApi.Infraestructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Pos.WebApi.Features.Liquidations.Services
{
    public class LiquidationServices
    {
        private readonly PosDbContext _context;
        private readonly IConfiguration _config;

        public LiquidationServices(PosDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
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
                              Comment = e.Comment,
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
            return GetLiquidationBase(x => x.From.Date >= From.Date && x.To.Date <= To.Date).ToList();
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

        public LiquidationSellerDto GetLiquidationSellerResums(int sellerId, DateTime date)
        {
            string connectionString = _config["connectionStrings:dbpos"];
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var parameters = new DynamicParameters();
                parameters.Add("@VendedorId", sellerId, DbType.Int32);
                parameters.Add("@FechaConsulta", date.Date, DbType.Date);
                LiquidationSellerDto result = new LiquidationSellerDto();
                result.Resum= connection.Query<LiquidationSellerResumDto>(
                    "sp_ObtenerLiquidacionVendedorResumen",
                    parameters,
                    commandType: CommandType.StoredProcedure
                ).AsList();

                result.Detail = connection.Query<LiquidationSellerResumDetailDto>(
                    "sp_ObtenerLiquidacionVendedorDetalle",
                    parameters,
                    commandType: CommandType.StoredProcedure
                ).AsList();

                return result;


            }
        }
        //Money

        public List<MoneyBill> GetMoneyBill()
        {
            return _context.MoneyBill.Where(x=> x.Activo).ToList();
        }

        public List<MoneyLiquidation> AddMoneyLiquidation(MoneyLiquidation request)
        {
            request.IsValid();
            _context.Database.BeginTransaction();
            try
            {
                request.Details.ForEach(x => x.LiquidationId = 0);
                request.CreatedAt = DateTime.Now;
 
                _context.MoneyLiquidation.Add(request);
                _context.SaveChanges();
                _context.Database.CommitTransaction();
            }
            catch (Exception ex)
            {
                _context.Database.RollbackTransaction();
                throw new Exception(ex.Message);
            }
            return GetMoneyLiquidationByDate(request.ExpenseDate, request.ExpenseDate);
        }

        public MoneyLiquidation UpdateMoneyLiquidation(MoneyLiquidation request)
        {
            request.IsValid();
            _context.Database.BeginTransaction();
            try
            {
                var existingLiquidation = _context.MoneyLiquidation
                    .Include(ml => ml.Details)
                    .FirstOrDefault(ml => ml.LiquidationId == request.LiquidationId);

                if (existingLiquidation == null)
                {
                    throw new Exception("Liquidación no encontrada");
                }

                // Actualizar propiedades principales
                existingLiquidation.ExpenseDate = request.ExpenseDate;
                existingLiquidation.Comment = request.Comment;
                existingLiquidation.Total = request.Total;
                existingLiquidation.Deposit = request.Deposit;
                existingLiquidation.SellerId = request.SellerId;
                existingLiquidation.UpdatedAt = DateTime.Now;
 

                // Actualizar detalles
                _context.MoneyLiquidationDetail.RemoveRange(existingLiquidation.Details);
                existingLiquidation.Details = request.Details.Select(d => new MoneyLiquidationDetail
                {
                    LiquidationId = existingLiquidation.LiquidationId,
                    MoneyId = d.MoneyId,
                    Denominacion = d.Denominacion,
                    Quantity = d.Quantity,
                    Total = d.Total
                }).ToList();

                _context.SaveChanges();
                _context.Database.CommitTransaction();

                return existingLiquidation;
            }
            catch (Exception ex)
            {
                _context.Database.RollbackTransaction();
                throw new Exception($"Error al actualizar la liquidación: {ex.Message}", ex);
            }
        }

        public List<MoneyLiquidation> GetMoneyLiquidationByDate(DateTime from, DateTime to)
        {
            return _context.MoneyLiquidation
                .Include(x=> x.Details)
                .Include(x=> x.Seller)
                .Where(x=> x.ExpenseDate.Date >= from.Date && x.ExpenseDate.Date<=to.Date)
                .OrderByDescending(x=> x.LiquidationId)
                .ToList();
        }
    }
}
