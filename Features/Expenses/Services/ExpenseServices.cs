using Pos.WebApi.Features.Expenses.Dto;
using Pos.WebApi.Features.Expenses.Entities;
using Pos.WebApi.Features.Items.Dto;
using Pos.WebApi.Features.Items.Entities;
using Pos.WebApi.Features.Purchase.Dto;
using Pos.WebApi.Features.Purchase.Entities;
using Pos.WebApi.Infraestructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pos.WebApi.Features.Expenses.Services
{
    public class ExpenseServices
    {
        private readonly PosDbContext _context;

        public ExpenseServices(PosDbContext posDbContext)
        {
            _context = posDbContext;
        }

        public List<ExpenseTypeDto> GetExpenseTypes()
        {
            var result = ( from expense in _context.ExpenseType
                           join user in _context.User on expense.CreatedBy equals user.UserId
            select( new ExpenseTypeDto
            {
                ExpenseTypeId = expense.ExpenseTypeId,
                ExpenseName = expense.ExpenseName,
                CreatedBy = expense.CreatedBy,
                CreatedByName = user.Name,
                CreatedDate = expense.CreatedDate
            })).ToList();
            return result;
        }

        public List<ExpenseTypeDto> AddExpenseType(ExpenseType expense)
        {
            expense.IsValid();
            expense.CreatedDate = DateTime.Now;
            _context.Add(expense);
            _context.SaveChanges();
            return GetExpenseTypes();
        }

        public List<ExpenseTypeDto> EditExpenseType(ExpenseType request)
        {
            request.IsValid();
            var currentItemCategory = _context.ExpenseType.Where(x => x.ExpenseTypeId == request.ExpenseTypeId).FirstOrDefault();
            currentItemCategory.ExpenseName = request.ExpenseName;
            currentItemCategory.UpdateBy = request.CreatedBy;
            currentItemCategory.UpdateDate = DateTime.Now;
            _context.SaveChanges();
            return GetExpenseTypes();
        }

        public List<ExpenseDto> GetBaseExpense(Func<Expense, bool> condition)
        {
            var expense= _context.Expense.Where(condition).ToList();
            var result = (from e in expense
                          join user in _context.User on e.CreatedBy equals user.UserId
                          join seller in _context.Seller on e.SellerId equals seller.SellerId
                          select new ExpenseDto
                          {
                              ExpenseId = e.ExpenseId,
                              ExpenseDate = e.ExpenseDate,
                              Comment = e.Comment,
                              Total = e.Total,
                              CreatedBy = e.CreatedBy,
                              CreateByName = user.Name,
                              SellerId = e.SellerId,
                              SellerName = seller.SellerName,
                              Active = e.Active,
                              Detail = (from d in _context.ExpenseDetail
                                        join expenseType in _context.ExpenseType on d.ExpenseTypeId equals expenseType.ExpenseTypeId
                                        where d.ExpenseId == e.ExpenseId && d.IsDeleted == false
                                        select new ExpenseDetailDto
                                        {
                                            ExpenseDetailId = d.ExpenseDetailId,
                                            ExpenseId = d.ExpenseId,
                                            ExpenseTypeName = expenseType.ExpenseName,
                                            ExpenseTypeId = d.ExpenseTypeId,
                                            Reference = d.Reference,
                                            LineTotal = d.LineTotal,
                                            IsDeleted = d.IsDeleted,
                                        }).ToList(),
                             
                          }).ToList();
            return result;
        }

        public List<ExpenseDto> GetExpenseByDate(DateTime From, DateTime To)
        {
            var result = GetBaseExpense(x => x.ExpenseDate.Date >= From.Date && x.ExpenseDate.Date <= To.Date).ToList();
            return result;
        }
        public List<ExpenseDto> GetExpenseById(int id)
        {
            var result = GetBaseExpense(x => x.ExpenseId == id).ToList();
            return result;
        }

        public List<ExpenseDto> AddExpense(Expense request)
        {
            request.IsValid();
            _context.Database.BeginTransaction();
            try
            {
                request.Detail.ForEach(x => x.ExpenseDetailId = 0);
                request.CreatedDate = DateTime.Now;
                request.Total = request.Detail.Sum(x => x.LineTotal);
                request.Active = true;
                _context.Expense.Add(request);
                _context.SaveChanges();
                _context.Database.CommitTransaction();
            }
            catch (Exception ex)
            {
                _context.Database.RollbackTransaction();
                throw new Exception(ex.Message);
            }
            return GetExpenseById(request.ExpenseId);
        }

        public List<ExpenseDto> EditExpense(Expense request)
        {
            request.IsValid();
            var currentInvoice = _context.Expense.Where(x => x.ExpenseId == request.ExpenseId).FirstOrDefault();
            if (currentInvoice == null) throw new Exception("No existe este gasto, comuniquese con el administrador del sistema.");
            _context.Database.BeginTransaction();
            try
            {
                currentInvoice.ExpenseDate = request.ExpenseDate;
                currentInvoice.SellerId = request.SellerId;
                currentInvoice.Comment = request.Comment;
                currentInvoice.CreatedBy = request.CreatedBy;
                currentInvoice.Total = request.Detail.Where(x=> x.IsDeleted==false).Sum(x => x.LineTotal);
                var currentDetail = _context.ExpenseDetail.Where(x => x.ExpenseId == currentInvoice.ExpenseId).ToList();
                var newLine = request.Detail.Where(x => x.ExpenseDetailId == 0).ToList();
                //Agregamos las nuevas lineas
                _context.ExpenseDetail.AddRange(newLine);
                //Actualizamos las existente
                currentDetail.ForEach((item) =>
                {
                    var itemToUpdate = request.Detail.Where(x => x.ExpenseDetailId == item.ExpenseDetailId && item.ExpenseId != 0).FirstOrDefault();
                    if (itemToUpdate != null)
                    {
                        item.ExpenseTypeId = itemToUpdate.ExpenseTypeId;
                        item.Reference = itemToUpdate.Reference;
                        item.LineTotal = itemToUpdate.LineTotal;
                        item.IsDeleted = itemToUpdate.IsDeleted;
                   }

                });
                _context.ExpenseDetail.UpdateRange(currentDetail);
                _context.Expense.Update(currentInvoice);
                _context.SaveChanges();
                _context.Database.CommitTransaction();
            }
            catch (Exception ex)
            {
                _context.Database.RollbackTransaction();
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
            return GetExpenseById(request.ExpenseId);
        }

        public List<ExpenseDto> CancelExpense(Expense request)
        {
            request.IsValid();
            var currentInvoice = _context.Expense.Where(x => x.ExpenseId == request.ExpenseId).FirstOrDefault();
            if (currentInvoice == null) throw new Exception("No existe este gasto, comuniquese con el administrador del sistema.");
            _context.Database.BeginTransaction();
            try
            {
                currentInvoice.Active = false;
                _context.Expense.Update(currentInvoice);
                _context.SaveChanges();
                _context.Database.CommitTransaction();
            }
            catch (Exception ex)
            {
                _context.Database.RollbackTransaction();
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
            return GetExpenseById(request.ExpenseId);
        }


    }
}
