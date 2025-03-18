using Microsoft.EntityFrameworkCore;
using Pos.WebApi.Features.InventoryTransactions.Dto;
using Pos.WebApi.Features.InventoryTransactions.Entities;
using Pos.WebApi.Features.Items.Entities;
using Pos.WebApi.Features.Items.Services;
using Pos.WebApi.Features.Reports.Services;
using Pos.WebApi.Infraestructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Pos.WebApi.Features.InventoryTransactions.Services
{
    public class InventoryTransactionServices
    {
        private readonly PosDbContext _context;
        private readonly ItemJournalServices _journalServices;
        private readonly WareHouseServices _wareHouseServices;
        private readonly ItemServices _itemServices;
        private readonly GenericDataService _dataService;

        public InventoryTransactionServices(PosDbContext context, ItemJournalServices journalServices, WareHouseServices wareHouseServices, ItemServices itemServices, GenericDataService dataServices)
        {
            _context = context;
            _journalServices = journalServices;
            _wareHouseServices = wareHouseServices;
            _itemServices = itemServices;
            _dataService = dataServices;
        }

        //Type
        public List<InventoryTransactionTypeDto> GetInventoryTransactionType()
        {
            var transactionType = _context.InventoryTransactionType.ToList();
            var userId = transactionType.Select(x => x.CreateBy).Distinct().ToList();
            var user = _context.User.Where(x => userId.Contains(x.UserId)).ToList();
            var result = (from c in transactionType
                          join u in user on c.CreateBy equals u.UserId
                          select new InventoryTransactionTypeDto
                          {
                              Id = c.Id,
                              Name = c.Name,
                              Transaction = c.Transaction,
                              CreateBy = c.CreateBy,
                              CreateByName = u.Name,
                              CreateDate = c.CreateDate,
                          }).ToList();
            return result;

        }

        public List<InventoryTransactionTypeDto> GetInventoryTransactionTypeBy(string type)
        {
            var transactionType = _context.InventoryTransactionType.Where(x=> x.Transaction==type).ToList();
            var userId = transactionType.Select(x => x.CreateBy).Distinct().ToList();
            var user = _context.User.Where(x => userId.Contains(x.UserId)).ToList();
            var result = (from c in transactionType
                          join u in user on c.CreateBy equals u.UserId
                          select new InventoryTransactionTypeDto
                          {
                              Id = c.Id,
                              Name = c.Name,
                              Transaction = c.Transaction,
                              CreateBy = c.CreateBy,
                              CreateByName = u.Name,
                              CreateDate = c.CreateDate,
                          }).ToList();
            return result;

        }

        public List<InventoryTransactionTypeDto> AddInventoryTransactionType(InventoryTransactionType request)
        {
            request.IsValid();
            request.CreateDate = DateTime.Now;
            _context.InventoryTransactionType.Add(request);
            _context.SaveChanges();
            return GetInventoryTransactionType();
        }

        public List<InventoryTransactionTypeDto> EditInventoryTransactionType(InventoryTransactionType request)
        {
            request.IsValid();
            var currentInventoryTransactionTypeDto = _context.InventoryTransactionType.Where(x => x.Id == request.Id).FirstOrDefault();
            currentInventoryTransactionTypeDto.Name = request.Name;
            currentInventoryTransactionTypeDto.Transaction = request.Transaction;
            _context.SaveChanges();
            return GetInventoryTransactionType();
        }

        //Entry
        public List<InventoryEntryDto> GetEntryById(int id)
        {
            var result = GetBase(x => x.EntryId == id).ToList();
            return result;
        }
        public List<InventoryEntryDto> GetEntryByDate(DateTime From, DateTime To)
        {
            var result = GetBase(x => x.EntryDate.Date >= From && x.EntryDate.Date <= To.Date).ToList();
            return result;
        }
        private List<InventoryEntryDto> GetBase(Func<InventoryEntry, bool> condition)
        {
            var entry = _context.InventoryEntry.Where(condition).ToList();
            var userId = entry.Select(x => x.CreateBy).Distinct().ToList();
            var user = _context.User.Where(x => userId.Contains(x.UserId)).ToList();
            var whsCode = entry.Select(x => x.WhsCode).Distinct().ToList();
            var whs = _context.WareHouse.Where(x => whsCode.Contains(x.WhsCode)).ToList();
            var entryId = entry.Select(x => x.EntryId).Distinct().ToList();
            var entryDetail = _context.InventoryEntryDetail.Where(x => entryId.Contains(x.EntryId)).ToList();
            var itemId = entryDetail.Select(x => x.ItemId).Distinct().ToList();
            var items = _context.Item.Where(x => itemId.Contains(x.ItemId)).ToList();
            var unit = entryDetail.Select(x => x.UnitOfMeasureId).Distinct().ToList();
            var units = _context.UnitOfMeasure.Where(x => unit.Contains(x.UnitOfMeasureId)).ToList();
            var transactionType = _context.InventoryTransactionType.ToList();

            var edetail = (from d in entryDetail
                          join i in items on d.ItemId equals i.ItemId
                          join w in whs on d.WhsCode equals w.WhsCode
                          join u in units on d.UnitOfMeasureId equals u.UnitOfMeasureId
                          select new InventoryEntryDetailDto
                          {
                              EntryDetailId = d.EntryDetailId,
                              EntryId = d.EntryId,
                              WhsCode = w.WhsCode,
                              WhsName = w.WhsName,
                              ItemCode = i.ItemCode,
                              ItemName = i.ItemName,
                              Quantity = d.Quantity,
                              UnitOfMeasureId = d.UnitOfMeasureId,
                              UnitOfMeasureName = u.UnitOfMeasureName,
                              Price = d.Price,
                              DueDate = d.DueDate,
                              LineTotal = d.LineTotal
                          }).ToList();

            var result = (from e in entry
                          join d in edetail on e.EntryId equals d.EntryId into detail
                          join w in whs on e.WhsCode equals w.WhsCode
                          join u in user on e.CreateBy equals u.UserId
                          join t in transactionType on e.Type equals t.Id
                          select new InventoryEntryDto
                          {
                              EntryId = e.EntryId,
                              EntryDate = e.EntryDate,
                              Comment = e.Comment,
                              DocTotal = e.DocTotal,
                              DocQuantity = detail.Sum(x => x.Quantity),
                              WhsCode = e.WhsCode,
                              WhsName = w.WhsName,
                              Type = e.Type,
                              TypeName = t.Name,
                              CreateBy = e.CreateBy,
                              CreateByName = u.Name,
                              Detail = detail.ToList()
                          }).ToList();
            return result.OrderByDescending(x => x.EntryDate).ToList();
        }
        public List<InventoryEntryDto> GetInventoryEntry()
        {
            var entry = _context.InventoryEntry.ToList();
            var userId = entry.Select(x => x.CreateBy).Distinct().ToList();
            var user = _context.User.Where(x => userId.Contains(x.UserId)).ToList();
            var whsCode = entry.Select(x => x.WhsCode).Distinct().ToList();
            var whs = _context.WareHouse.Where(x => whsCode.Contains(x.WhsCode)).ToList();
            var entryId = entry.Select(x => x.EntryId).Distinct().ToList();
            var entryDetail = _context.InventoryEntryDetail.Where(x => entryId.Contains(x.EntryId)).ToList();
            var itemId = entryDetail.Select(x => x.ItemId).Distinct().ToList();
            var items = _context.Item.Where(x => itemId.Contains(x.ItemId)).ToList();
            var unit = entryDetail.Select(x => x.UnitOfMeasureId).Distinct().ToList();
            var units = _context.UnitOfMeasure.Where(x => unit.Contains(x.UnitOfMeasureId)).ToList();
            var transactionType = _context.InventoryTransactionType.ToList();

            var edetail = (from d in entryDetail
                           join i in items on d.ItemId equals i.ItemId
                           join w in whs on d.WhsCode equals w.WhsCode
                           join u in units on d.UnitOfMeasureId equals u.UnitOfMeasureId
                           select new InventoryEntryDetailDto
                           {
                               EntryDetailId = d.EntryDetailId,
                               EntryId = d.EntryId,
                               WhsCode = w.WhsCode,
                               WhsName = w.WhsName,
                               ItemCode = i.ItemCode,
                               ItemName = i.ItemName,
                               Quantity = d.Quantity,
                               UnitOfMeasureId = d.UnitOfMeasureId,
                               UnitOfMeasureName = u.UnitOfMeasureName,
                               Price = d.Price,
                               DueDate = d.DueDate,
                               LineTotal = d.LineTotal
                           }).ToList();

            var result = (from e in entry
                          join d in edetail on e.EntryId equals d.EntryId into detail
                          join w in whs on e.WhsCode equals w.WhsCode
                          join u in user on e.CreateBy equals u.UserId
                          join t in transactionType on e.Type equals t.Id
                          select new InventoryEntryDto
                          {
                              EntryId = e.EntryId,
                              EntryDate = e.EntryDate,
                              Comment = e.Comment,
                              DocTotal = e.DocTotal,
                              DocQuantity = detail.Sum(x=> x.Quantity),
                              WhsCode = e.WhsCode,
                              WhsName = w.WhsName,
                              Type = e.Type,
                              TypeName = t.Name,
                              CreateBy = e.CreateBy,
                              CreateByName = u.Name,
                              Detail = detail.ToList()
                          }).ToList();
            return result.OrderByDescending(x => x.EntryDate).ToList();
        }
        public List<InventoryEntryDto> AddInventoryEntry(InventoryEntry request)
        {
            request.IsValid();
            _context.Database.BeginTransaction();
            try
            {
                DateTime fechaPorDefecto = DateTime.MinValue;
                request.EntryDate = request.EntryDate != fechaPorDefecto ? request.EntryDate : DateTime.Now;
                request.CreateDate = DateTime.Now;
                request.DocTotal = request.Detail.Sum(x => x.LineTotal);
                _context.InventoryEntry.Add(request);
                _context.SaveChanges();
                var journal = request.Detail.Select(x => new ItemJournal
                {
                    ItemId = x.ItemId,
                    WhsCode = x.WhsCode,
                    Quantity = x.Quantity,
                    Price = x.Price,
                    TransValue = x.Price * x.Quantity,
                    Documents = "Entrada de mercancia",
                    DocumentReferent = request.EntryId,
                    CreateBy = request.CreateBy,
                    CreateDate = request.EntryDate
                }).ToList();
                _journalServices.AddLinesJournal(journal);
                var warehouse = request.Detail.Select(x => new ItemWareHouse
                {
                    ItemId = x.ItemId,
                    WhsCode = x.WhsCode,
                    Stock = x.Quantity,
                    AvgPrice = x.Price,
                    CreateDate = request.EntryDate,
                    DueDate = x.DueDate
                }).ToList();
                warehouse.ForEach(x=> _wareHouseServices.UpdateItemWareHouse(x));
                warehouse.ForEach(x=> _itemServices.UpdateStockItem(x.ItemId));
                _context.Database.CommitTransaction();
            }
            catch (Exception ex)
            {
                _context.Database.RollbackTransaction();
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
            return GetEntryById(request.EntryId);
        }
        //OutPut
        public List<InventoryOutPutDto> GetOutPutById(int id)
        {
            var result = GetBase(x => x.OutputId == id).ToList();
            return result;
        }
        public List<InventoryOutPutDto> GetOutPutByDate(DateTime From, DateTime To)
        {
            var result = GetBase(x => x.OutputDate.Date >= From && x.OutputDate.Date <= To.Date).ToList();
            return result;
        }
        private List<InventoryOutPutDto> GetBase(Func<InventoryOutPut, bool> condition)
        {
            var outPut = _context.InventoryOutPut.Where(condition).ToList();
            var userId = outPut.Select(x => x.CreateBy).Distinct().ToList();
            var user = _context.User.Where(x => userId.Contains(x.UserId)).ToList();
            var whsCode = outPut.Select(x => x.WhsCode).Distinct().ToList();
            var whs = _context.WareHouse.Where(x => whsCode.Contains(x.WhsCode)).ToList();
            var outPutId = outPut.Select(x => x.OutputId).Distinct().ToList();
            var outPutDetail = _context.InventoryOutPutDetail.Where(x => outPutId.Contains(x.OutputId)).ToList();
            var itemId = outPutDetail.Select(x => x.ItemId).Distinct().ToList();
            var items = _context.Item.Where(x => itemId.Contains(x.ItemId)).ToList();
            var unit = outPutDetail.Select(x => x.UnitOfMeasureId).Distinct().ToList();
            var units = _context.UnitOfMeasure.Where(x => unit.Contains(x.UnitOfMeasureId)).ToList();
            var transactionType = _context.InventoryTransactionType.ToList();

            var edetail = (from d in outPutDetail
                           join i in items on d.ItemId equals i.ItemId
                           join w in whs on d.WhsCode equals w.WhsCode
                           join u in units on d.UnitOfMeasureId equals u.UnitOfMeasureId
                           select new InventoryOutPutDetailDto
                           {
                               OutputDetailId = d.OutputDetailId,
                               OutputId = d.OutputId,
                               WhsCode = w.WhsCode,
                               WhsName = w.WhsName,
                               ItemCode = i.ItemCode,
                               ItemName = i.ItemName,
                               Quantity = d.Quantity,
                               UnitOfMeasureId = d.UnitOfMeasureId,
                               UnitOfMeasureName = u.UnitOfMeasureName,
                               Price = d.Price,
                               DueDate = d.DueDate,
                               LineTotal = d.LineTotal
                           }).ToList();

            var result = (from e in outPut
                          join d in edetail on e.OutputId equals d.OutputId into detail
                          join w in whs on e.WhsCode equals w.WhsCode
                          join u in user on e.CreateBy equals u.UserId
                          join t in transactionType on e.Type equals t.Id
                          select new InventoryOutPutDto
                          {
                              OutputId = e.OutputId,
                              OutputDate = e.OutputDate,
                              Comment = e.Comment,
                              DocTotal = e.DocTotal,
                              DocQuantity = detail.Sum(x => x.Quantity),
                              WhsCode = e.WhsCode,
                              WhsName = w.WhsName,
                              Type = e.Type,
                              TypeName = t.Name,
                              CreateBy = e.CreateBy,
                              CreateByName = u.Name,
                              Detail = detail.ToList()
                          }).ToList();
            return result.OrderByDescending(x => x.OutputDate).ToList();
        }
        public List<InventoryOutPutDto> GetInventoryOutPut()
        {
            var outPut = _context.InventoryOutPut.ToList();
            var userId = outPut.Select(x => x.CreateBy).Distinct().ToList();
            var user = _context.User.Where(x => userId.Contains(x.UserId)).ToList();
            var whsCode = outPut.Select(x => x.WhsCode).Distinct().ToList();
            var whs = _context.WareHouse.Where(x => whsCode.Contains(x.WhsCode)).ToList();
            var outPutId = outPut.Select(x => x.OutputId).Distinct().ToList();
            var outPutDetail = _context.InventoryOutPutDetail.Where(x => outPutId.Contains(x.OutputId)).ToList();
            var itemId = outPutDetail.Select(x => x.ItemId).Distinct().ToList();
            var items = _context.Item.Where(x => itemId.Contains(x.ItemId)).ToList();
            var unit = outPutDetail.Select(x => x.UnitOfMeasureId).Distinct().ToList();
            var units = _context.UnitOfMeasure.Where(x => unit.Contains(x.UnitOfMeasureId)).ToList();
            var transactionType = _context.InventoryTransactionType.ToList();

            var edetail = (from d in outPutDetail
                           join i in items on d.ItemId equals i.ItemId
                           join w in whs on d.WhsCode equals w.WhsCode
                           join u in units on d.UnitOfMeasureId equals u.UnitOfMeasureId
                           select new InventoryOutPutDetailDto
                           {
                               OutputDetailId = d.OutputDetailId,
                               OutputId = d.OutputId,
                               WhsCode = w.WhsCode,
                               WhsName = w.WhsName,
                               ItemCode = i.ItemCode,
                               ItemName = i.ItemName,
                               Quantity = d.Quantity,
                               UnitOfMeasureId = d.UnitOfMeasureId,
                               UnitOfMeasureName = u.UnitOfMeasureName,
                               Price = d.Price,
                               DueDate = d.DueDate,
                               LineTotal = d.LineTotal
                           }).ToList();

            var result = (from e in outPut
                          join d in edetail on e.OutputId equals d.OutputId into detail
                          join w in whs on e.WhsCode equals w.WhsCode
                          join u in user on e.CreateBy equals u.UserId
                          join t in transactionType on e.Type equals t.Id
                          select new InventoryOutPutDto
                          {
                              OutputId = e.OutputId,
                              OutputDate = e.OutputDate,
                              Comment = e.Comment,
                              DocTotal = e.DocTotal,
                              DocQuantity = detail.Sum(x => x.Quantity),
                              WhsCode = e.WhsCode,
                              WhsName = w.WhsName,
                              Type = e.Type,
                              TypeName = t.Name,
                              CreateBy = e.CreateBy,
                              CreateByName = u.Name,
                              Detail = detail.ToList()
                          }).ToList();
            return result.OrderByDescending(x => x.OutputDate).ToList();
        }
        public List<InventoryOutPutDto> AddInventoryOutPut(InventoryOutPut request)
        {
            request.IsValid();
            _context.Database.BeginTransaction();
            try
            {
                DateTime fechaPorDefecto = DateTime.MinValue;
                request.OutputDate = request.OutputDate != fechaPorDefecto ? request.OutputDate : DateTime.Now;
                request.CreatedDate = DateTime.Now;
                request.DocTotal = request.Detail.Sum(x => x.LineTotal);
                _context.InventoryOutPut.Add(request);
                _context.SaveChanges();
                var journal = request.Detail.Select(x => new ItemJournal
                {
                    ItemId = x.ItemId,
                    WhsCode = x.WhsCode,
                    Quantity = x.Quantity * -1,
                    Price = x.Price,
                    TransValue = (x.Price * x.Quantity) * -1,
                    Documents = "Salida de mercancia",
                    DocumentReferent = request.OutputId,
                    CreateBy = request.CreateBy,
                    CreateDate = request.OutputDate

                }).ToList();
                _journalServices.AddLinesJournal(journal);
                var warehouse = request.Detail.Select(x => new ItemWareHouse
                {
                    ItemId = x.ItemId,
                    WhsCode = x.WhsCode,
                    Stock = x.Quantity * -1,
                    AvgPrice = x.Price,
                    CreateDate = request.OutputDate,
                    DueDate = x.DueDate
                }).ToList();
                warehouse.ForEach(x => _wareHouseServices.UpdateItemWareHouse(x));
                warehouse.ForEach(x => _itemServices.UpdateStockItem(x.ItemId));
                _context.Database.CommitTransaction();
            }
            catch (Exception ex)
            {
                _context.Database.RollbackTransaction();
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
            return GetOutPutById(request.OutputId);
        }
        //Transfer
        public List<InventoryTransferDto> GetTransferById(int id)
        {
            var result = GetBase(x => x.TransferId == id).ToList();
            return result;
        }
        public List<InventoryTransferDto> GetTransferByDate(DateTime From, DateTime To)
        {
            var result = GetBase(x => x.TransferDate.Date >= From && x.TransferDate.Date <= To.Date).ToList();
            return result;
        }
        private List<InventoryTransferDto> GetBase(Func<InventoryTransfer, bool> condition)
        {
            var transfer = _context.InventoryTransfer.Where(condition).ToList();
            var userId = transfer.Select(x => x.CreateBy).Distinct().ToList();
            var user = _context.User.Where(x => userId.Contains(x.UserId)).ToList();
            var fromwhsCode = transfer.Select(x => x.FromWhsCode).Distinct().ToList();
            var fromWhs = _context.WareHouse.Where(x => fromwhsCode.Contains(x.WhsCode)).ToList();
            var tomwhsCode = transfer.Select(x => x.ToWhsCode).Distinct().ToList();
            var tomWhs = _context.WareHouse.Where(x => tomwhsCode.Contains(x.WhsCode)).ToList();
            var transferId = transfer.Select(x => x.TransferId).Distinct().ToList();
            var transferDetail = _context.InventoryTransferDetail.Where(x => transferId.Contains(x.TransferId)).ToList();
            var itemId = transferDetail.Select(x => x.ItemId).Distinct().ToList();
            var items = _context.Item.Where(x => itemId.Contains(x.ItemId)).ToList();
            var unit = transferDetail.Select(x => x.UnitOfMeasureId).Distinct().ToList();
            var units = _context.UnitOfMeasure.Where(x => unit.Contains(x.UnitOfMeasureId)).ToList();
            var edetail = (from d in transferDetail
                           join i in items on d.ItemId equals i.ItemId
                           join tw in tomWhs on d.ToWhsCode equals tw.WhsCode
                           join fw in fromWhs on d.FromWhsCode equals fw.WhsCode
                           join u in units on d.UnitOfMeasureId equals u.UnitOfMeasureId
                           select new InventoryTransferDetailDto
                           {
                               TransferDetailId = d.TransferDetailId,
                               TransferId = d.TransferId,
                               FromWhsCode = fw.WhsCode,
                               FromWhsName = fw.WhsName,
                               ToWhsCode = tw.WhsCode,
                               ToWhsName = tw.WhsName,
                               ItemCode = i.ItemCode,
                               ItemName = i.ItemName,
                               Quantity = d.Quantity,
                               QuantityUnit = d.QuantityUnit,
                               UnitOfMeasureName = u.UnitOfMeasureName,
                               UnitOfMeasureId = u.UnitOfMeasureId,
                               Price = d.Price,
                               DueDate = d.DueDate,
                               LineTotal = d.LineTotal
                           }).ToList();

            var result = (from e in transfer
                          join d in edetail on e.TransferId equals d.TransferId into detail
                          join tw in tomWhs on e.ToWhsCode equals tw.WhsCode
                          join fw in fromWhs on e.FromWhsCode equals fw.WhsCode
                          join u in user on e.CreateBy equals u.UserId
                          select new InventoryTransferDto
                          {
                              TransferId = e.TransferId,
                              TransferDate = e.TransferDate,
                              Comment = e.Comment,
                              DocTotal = e.DocTotal,
                              QtyTotal = e.QtyTotal,
                              FromWhsCode = fw.WhsCode,
                              FromWhsName = fw.WhsName,
                              ToWhsName= tw.WhsName,
                              ToWhsCode = tw.WhsCode,
                              CreateBy = e.CreateBy,
                              CreateByName = u.Name,
                              Detail = detail.ToList()
                          }).ToList();
            return result.OrderByDescending(x => x.TransferDate).ToList();
        }
        public List<InventoryTransferDto> GetInventoryTransfer()
        {
            var transfer = _context.InventoryTransfer.ToList();
            var userId = transfer.Select(x => x.CreateBy).Distinct().ToList();
            var user = _context.User.Where(x => userId.Contains(x.UserId)).ToList();
            var fromwhsCode = transfer.Select(x => x.FromWhsCode).Distinct().ToList();
            var fromWhs = _context.WareHouse.Where(x => fromwhsCode.Contains(x.WhsCode)).ToList();
            var tomwhsCode = transfer.Select(x => x.ToWhsCode).Distinct().ToList();
            var tomWhs = _context.WareHouse.Where(x => tomwhsCode.Contains(x.WhsCode)).ToList();
            var transferId = transfer.Select(x => x.TransferId).Distinct().ToList();
            var transferDetail = _context.InventoryTransferDetail.Where(x => transferId.Contains(x.TransferId)).ToList();
            var itemId = transferDetail.Select(x => x.ItemId).Distinct().ToList();
            var items = _context.Item.Where(x => itemId.Contains(x.ItemId)).ToList();
            var unit = transferDetail.Select(x => x.UnitOfMeasureId).Distinct().ToList();
            var units = _context.UnitOfMeasure.Where(x => unit.Contains(x.UnitOfMeasureId)).ToList();

            var edetail = (from d in transferDetail
                           join i in items on d.ItemId equals i.ItemId
                           join tw in tomWhs on d.ToWhsCode equals tw.WhsCode
                           join fw in fromWhs on d.FromWhsCode equals fw.WhsCode
                           join u in units on d.UnitOfMeasureId equals u.UnitOfMeasureId
                           select new InventoryTransferDetailDto
                           {
                               TransferDetailId = d.TransferDetailId,
                               TransferId = d.TransferId,
                               FromWhsCode = fw.WhsCode,
                               FromWhsName = fw.WhsName,
                               ToWhsCode = tw.WhsCode,
                               ToWhsName = tw.WhsName,
                               ItemCode = i.ItemCode,
                               ItemName = i.ItemName,
                               Quantity = d.Quantity,
                               QuantityUnit = d.QuantityUnit,
                               UnitOfMeasureName = u.UnitOfMeasureName,
                               UnitOfMeasureId = u.UnitOfMeasureId,
                               Price = d.Price,
                               DueDate = d.DueDate,
                               LineTotal = d.LineTotal
                           }).ToList();

            var result = (from e in transfer
                          join d in edetail on e.TransferId equals d.TransferId into detail
                          join tw in tomWhs on e.ToWhsCode equals tw.WhsCode
                          join fw in fromWhs on e.FromWhsCode equals fw.WhsCode
                          join u in user on e.CreateBy equals u.UserId
                          select new InventoryTransferDto
                          {
                              TransferId = e.TransferId,
                              TransferDate = e.TransferDate,
                              Comment = e.Comment,
                              DocTotal = e.DocTotal,
                              QtyTotal = e.QtyTotal,
                              FromWhsCode = fw.WhsCode,
                              FromWhsName = fw.WhsName,
                              ToWhsName = tw.WhsName,
                              ToWhsCode = tw.WhsCode,
                              CreateBy = e.CreateBy,
                              CreateByName = u.Name,
                              Detail = detail.ToList()
                          }).ToList();
            return result.OrderByDescending(x => x.TransferDate).ToList();
        }
        public List<InventoryTransferDto> AddInventoryTransfer(InventoryTransfer request)
        {
            request.IsValid();
            _context.Database.BeginTransaction();
            try
            {
                DateTime fechaPorDefecto = DateTime.MinValue;
                request.TransferDate = request.TransferDate != fechaPorDefecto ? request.TransferDate : DateTime.Now;
                request.Detail = UpdateCost(request.Detail);
                request.Detail.ForEach(x => x.LineTotal = (x.Quantity * x.Price));
                request.DocTotal = request.Detail.Sum(x => x.LineTotal);
                
                _context.InventoryTransfer.Add(request);
                _context.SaveChanges();
                var journalToWhsCode = request.Detail.Select(x => new ItemJournal
                {
                    ItemId = x.ItemId,
                    WhsCode = x.ToWhsCode,
                    Quantity = x.Quantity,
                    Price = x.Price,
                    TransValue = (x.Price * x.Quantity),
                    Documents = "Transferencia de mercancia",
                    DocumentReferent = request.TransferId,
                    CreateBy = request.CreateBy,
                    CreateDate = request.TransferDate

                }).ToList();
                var journalFromWhsCode = request.Detail.Select(x => new ItemJournal
                {
                    ItemId = x.ItemId,
                    WhsCode = x.FromWhsCode,
                    Quantity = x.Quantity * -1,
                    Price = x.Price,
                    TransValue = (x.Price * x.Quantity) * -1,
                    Documents = "Transferencia de mercancia",
                    DocumentReferent = request.TransferId,
                    CreateBy = request.CreateBy,
                    CreateDate = request.TransferDate

                }).ToList();
                _journalServices.AddLinesJournal(journalToWhsCode);
                _journalServices.AddLinesJournal(journalFromWhsCode);
                var towarehouse = request.Detail.Select(x => new ItemWareHouse
                {
                    ItemId = x.ItemId,
                    WhsCode = x.ToWhsCode,
                    Stock = x.Quantity,
                    AvgPrice = x.Price,
                    CreateDate = DateTime.Now,
                    DueDate = x.DueDate
                }).ToList();
                var fromwarehouse = request.Detail.Select(x => new ItemWareHouse
                {
                    ItemId = x.ItemId,
                    WhsCode = x.FromWhsCode,
                    Stock = x.Quantity * -1,
                    AvgPrice = x.Price,
                    CreateDate = DateTime.Now,
                    DueDate = x.DueDate
                }).ToList();

                towarehouse.ForEach(x => _wareHouseServices.UpdateItemWareHouse(x));
                towarehouse.ForEach(x => _itemServices.UpdateStockItem(x.ItemId));
                fromwarehouse.ForEach(x => _wareHouseServices.UpdateItemWareHouse(x));
                fromwarehouse.ForEach(x => _itemServices.UpdateStockItem(x.ItemId));
                if(request.TransferRequestId != 0)
                    CompleteInventoryRequestTransfer(request.TransferRequestId);
                _context.Database.CommitTransaction();
            }
            catch (Exception ex)
            {
                _context.Database.RollbackTransaction();
                throw new Exception(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
            return GetTransferById(request.TransferId);
        }
        //Revaluation Cost
        public List<CostRevaluationDto> GetRevaluationCostBase(Func<CostRevaluation, bool> condition)
        {
            var revaluation = _context.CostRevaluation.Where(condition).ToList();
            var userId = revaluation.Select(x => x.CreateBy).Distinct().ToList();
            var users = _context.User.Where(x => userId.Contains(x.UserId)).ToList();
            var itemId = revaluation.Select(x => x.ItemId).Distinct().ToList();
            var items = _context.Item.Where(x => itemId.Contains(x.ItemId)).ToList();
            var whsId = revaluation.Select(x => x.WhsCode).Distinct().ToList();
            var whs = _context.WareHouse.Where(x => whsId.Contains(x.WhsCode)).ToList();

            var result = (from r in revaluation
                          join u in users on r.CreateBy equals u.UserId
                          join i in items on r.ItemId equals i.ItemId
                          join w in whs on r.WhsCode equals w.WhsCode
                          select new CostRevaluationDto
                          {
                              Id = r.Id,
                              ItemId = r.ItemId,
                              ItemCode = i.ItemCode,
                              ItemName = i.ItemName,
                              PreviousCost = r.PreviousCost,
                              NewCost = r.NewCost,
                              WhsCode = r.WhsCode,
                              WhsName = w.WhsName,
                              CreateBy = r.CreateBy,
                              CreateByName = u.Name,
                              CreateDate = r.CreateDate,
                              Comment = r.Comment

                          }).ToList();
            return result;
        }

        public List<CostRevaluationDto> GetRevaluationCostByDate(DateTime From, DateTime To)
        {
            var result = GetRevaluationCostBase(x => x.CreateDate.Date >= From && x.CreateDate.Date <= To.Date).ToList();
            return result;
        }

        public List<CostRevaluationDto> AddRevaluationCost(CostRevaluation request)
        {
            var itemWarehouse = _context.ItemWareHouse.Where(x => x.ItemId == request.ItemId && x.WhsCode == request.WhsCode).FirstOrDefault();
            itemWarehouse.AvgPrice = request.NewCost;
            _context.ItemWareHouse.Update(itemWarehouse);
            request.CreateDate = DateTime.Now;
            _context.CostRevaluation.Add(request);
            _context.SaveChanges();
            return GetRevaluationCostByDate(request.CreateDate, request.CreateDate);
        }
        //Request Transfer
        public async Task<List<Dictionary<string, object>>> GetItemsToTransfer(int almacenOrigen, int almacenDestino)
        {
            var parameters = new { AlmacenOrigen = almacenOrigen, AlmacenDestino = almacenDestino };
            var result = await _dataService.ExecuteStoredProcedureAsync("sp_ObtenerInfoTraslado", parameters);
            return result;
        }
        public List<InventoryRequestTransferDto> GetRequestTransferById(int id)
        {
            var result = GetBaseRequest(x => x.TransferRequestId == id).ToList();
            return result;
        }
        public List<InventoryRequestTransferDto> GetRequestTransferToComplete()
        {
            var result = GetBaseRequest(x => x.Complete== false).ToList();
            return result;
        }
        public List<InventoryRequestTransferDto> GetRequestTransferByDate(DateTime From, DateTime To, int userId)
        {
            var result = GetBaseRequest(x => x.TransferRequestDate.Date >= From && x.TransferRequestDate.Date <= To.Date && x.CreateBy == userId).ToList();
            return result;
        }
        private List<InventoryRequestTransferDto> GetBaseRequest(Func<InventoryRequestTransfer, bool> condition)
        {
            var transfer = _context.InventoryRequestTransfer.Where(condition).ToList();
            var userId = transfer.Select(x => x.CreateBy).Distinct().ToList();
            var user = _context.User.Where(x => userId.Contains(x.UserId)).ToList();
            var fromwhsCode = transfer.Select(x => x.FromWhsCode).Distinct().ToList();
            var fromWhs = _context.WareHouse.Where(x => fromwhsCode.Contains(x.WhsCode)).ToList();
            var tomwhsCode = transfer.Select(x => x.ToWhsCode).Distinct().ToList();
            var tomWhs = _context.WareHouse.Where(x => tomwhsCode.Contains(x.WhsCode)).ToList();
            var transferId = transfer.Select(x => x.TransferRequestId).Distinct().ToList();
            var transferDetail = _context.InventoryRequestTransferDetail.Where(x => transferId.Contains(x.TransferRequestId)).ToList();
            var itemId = transferDetail.Select(x => x.ItemId).Distinct().ToList();
            var items = _context.Item.Where(x => itemId.Contains(x.ItemId)).ToList();
            var unit = transferDetail.Select(x => x.UnitOfMeasureId).Distinct().ToList();
            var units = _context.UnitOfMeasure.Where(x => unit.Contains(x.UnitOfMeasureId)).ToList();
            var edetail = (from d in transferDetail
                           join i in items on d.ItemId equals i.ItemId
                           join tw in tomWhs on d.ToWhsCode equals tw.WhsCode
                           join fw in fromWhs on d.FromWhsCode equals fw.WhsCode
                           join u in units on d.UnitOfMeasureId equals u.UnitOfMeasureId
                           select new InventoryRequestTransferDetailDto
                           {
                               TransferRequestDetailId = d.TransferRequestDetailId,
                               TransferRequestId = d.TransferRequestId,
                               FromWhsCode = fw.WhsCode,
                               FromWhsName = fw.WhsName,
                               ToWhsCode = tw.WhsCode,
                               ToWhsName = tw.WhsName,
                               ItemId = i.ItemId,
                               ItemCode = i.ItemCode,
                               ItemName = i.ItemName,
                               Quantity = d.Quantity,
                               QuantityUnit = d.QuantityUnit,
                               UnitOfMeasureName = u.UnitOfMeasureName,
                               UnitOfMeasureId = u.UnitOfMeasureId,
                               Price = d.Price,
                               DueDate = d.DueDate,
                               LineTotal = d.LineTotal
                           }).ToList();

            var result = (from e in transfer
                          join d in edetail on e.TransferRequestId equals d.TransferRequestId into detail
                          join tw in tomWhs on e.ToWhsCode equals tw.WhsCode
                          join fw in fromWhs on e.FromWhsCode equals fw.WhsCode
                          join u in user on e.CreateBy equals u.UserId
                          select new InventoryRequestTransferDto
                          {
                              TransferRequestId = e.TransferRequestId,
                              TransferRequestDate = e.TransferRequestDate,
                              Comment = e.Comment,
                              DocTotal = e.DocTotal,
                              QtyTotal = e.QtyTotal,
                              FromWhsCode = fw.WhsCode,
                              FromWhsName = fw.WhsName,
                              ToWhsName = tw.WhsName,
                              ToWhsCode = tw.WhsCode,
                              CreateBy = e.CreateBy,
                              CreateByName = u.Name,
                              Complete = e.Complete,
                              Detail = detail.ToList()
                          }).ToList();
            return result.OrderByDescending(x => x.TransferRequestDate).ToList();
        }
        public List<InventoryRequestTransferDto> GetInventoryRequestTransfer()
        {
            var transfer = _context.InventoryRequestTransfer.ToList();
            var userId = transfer.Select(x => x.CreateBy).Distinct().ToList();
            var user = _context.User.Where(x => userId.Contains(x.UserId)).ToList();
            var fromwhsCode = transfer.Select(x => x.FromWhsCode).Distinct().ToList();
            var fromWhs = _context.WareHouse.Where(x => fromwhsCode.Contains(x.WhsCode)).ToList();
            var tomwhsCode = transfer.Select(x => x.ToWhsCode).Distinct().ToList();
            var tomWhs = _context.WareHouse.Where(x => tomwhsCode.Contains(x.WhsCode)).ToList();
            var transferId = transfer.Select(x => x.TransferRequestId).Distinct().ToList();
            var transferDetail = _context.InventoryRequestTransferDetail.Where(x => transferId.Contains(x.TransferRequestId)).ToList();
            var itemId = transferDetail.Select(x => x.ItemId).Distinct().ToList();
            var items = _context.Item.Where(x => itemId.Contains(x.ItemId)).ToList();
            var unit = transferDetail.Select(x => x.UnitOfMeasureId).Distinct().ToList();
            var units = _context.UnitOfMeasure.Where(x => unit.Contains(x.UnitOfMeasureId)).ToList();

            var edetail = (from d in transferDetail
                           join i in items on d.ItemId equals i.ItemId
                           join tw in tomWhs on d.ToWhsCode equals tw.WhsCode
                           join fw in fromWhs on d.FromWhsCode equals fw.WhsCode
                           join u in units on d.UnitOfMeasureId equals u.UnitOfMeasureId
                           select new InventoryRequestTransferDetailDto
                           {
                               TransferRequestDetailId = d.TransferRequestDetailId,
                               TransferRequestId = d.TransferRequestId,
                               FromWhsCode = fw.WhsCode,
                               FromWhsName = fw.WhsName,
                               ToWhsCode = tw.WhsCode,
                               ToWhsName = tw.WhsName,
                               ItemCode = i.ItemCode,
                               ItemName = i.ItemName,
                               Quantity = d.Quantity,
                               QuantityUnit = d.QuantityUnit,
                               UnitOfMeasureName = u.UnitOfMeasureName,
                               UnitOfMeasureId = u.UnitOfMeasureId,
                               Price = d.Price,
                               DueDate = d.DueDate,
                               LineTotal = d.LineTotal
                           }).ToList();

            var result = (from e in transfer
                          join d in edetail on e.TransferRequestId equals d.TransferRequestId into detail
                          join tw in tomWhs on e.ToWhsCode equals tw.WhsCode
                          join fw in fromWhs on e.FromWhsCode equals fw.WhsCode
                          join u in user on e.CreateBy equals u.UserId
                          select new InventoryRequestTransferDto
                          {
                              TransferRequestId = e.TransferRequestId,
                              TransferRequestDate = e.TransferRequestDate,
                              Comment = e.Comment,
                              DocTotal = e.DocTotal,
                              QtyTotal = e.QtyTotal,
                              FromWhsCode = fw.WhsCode,
                              FromWhsName = fw.WhsName,
                              ToWhsName = tw.WhsName,
                              ToWhsCode = tw.WhsCode,
                              CreateBy = e.CreateBy,
                              CreateByName = u.Name,
                              Detail = detail.ToList()
                          }).ToList();
            return result.OrderByDescending(x => x.TransferRequestDate).ToList();
        }
        public List<InventoryRequestTransferDto> AddInventoryRequestTransfer(InventoryRequestTransfer request)
        {
            request.IsValid();
            _context.Database.BeginTransaction();
            try
            {
                request.TransferRequestDate = DateTime.Now;
                request.DocTotal = request.Detail.Sum(x => x.LineTotal);
                _context.InventoryRequestTransfer.Add(request);
                _context.SaveChanges();
                _context.Database.CommitTransaction();
            }
            catch (Exception ex)
            {
                _context.Database.RollbackTransaction();
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                throw new Exception(mensaje);

            }
            return GetRequestTransferById(request.TransferRequestId);
        }

        public List<InventoryRequestTransferDto> UpdateInventoryRequestTransfer(InventoryRequestTransfer request)
        {
            try
            {
                // Buscar la entidad que se desea actualizar
                var existingInventoryReturn = _context.InventoryRequestTransfer.Include(ir => ir.Detail).FirstOrDefault(x => x.TransferRequestId == request.TransferRequestId);
                if (existingInventoryReturn != null)
                {
                    existingInventoryReturn.Comment = request.Comment;
                    existingInventoryReturn.ToWhsCode = request.ToWhsCode;
                    // Manejo del detalle
                    if (request.Detail != null)
                    {

                        // Elimina detalles antiguos
                        _context.InventoryRequestTransferDetail.RemoveRange(existingInventoryReturn.Detail);
                        existingInventoryReturn.Detail.Clear();

                        // Agrega los nuevos detalles
                        foreach (var detail in request.Detail)
                        {
                            detail.TransferRequestDetailId = 0;
                            detail.TransferRequestId = request.TransferRequestId;
                            existingInventoryReturn.Detail.Add(detail);
                        }
                    }
                    _context.InventoryRequestTransferDetail.AddRange(existingInventoryReturn.Detail);
                    _context.InventoryRequestTransfer.Update(existingInventoryReturn);
                    int affectedRows = _context.SaveChanges();

                    if (affectedRows == 0)
                    {
                        throw new DbUpdateConcurrencyException("No rows were affected. The entity may have been modified or deleted.");
                    }
                }
                else
                {
                    throw new Exception("Entity not found.");
                }

                return GetRequestTransferById(request.TransferRequestId);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // Manejo específico de excepciones de concurrencia
                var mensaje = "Concurrency conflict: " + ex.Message;
                throw new Exception(mensaje);
            }
            catch (Exception ex)
            {
                // Manejo de otras excepciones
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                throw new Exception(mensaje);
            }
        }

        public string CompleteInventoryRequestTransfer(int transferRequestId)
        {
            try
            {
                var currentTransferRequest = _context.InventoryRequestTransfer.Where(x => x.TransferRequestId == transferRequestId).FirstOrDefault();
                currentTransferRequest.Complete = true;
                _context.SaveChanges();
                return "OK";
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }

        public List<InventoryTransferDetail> UpdateCost(List<InventoryTransferDetail> detail)
        {
            detail.ForEach(x => x.Price = GetPriceItem(x.ItemId, x.FromWhsCode));
            return detail;
        }

        public decimal GetPriceItem(int itemId, int whsCode)
        {
            decimal price = 0;
            price = _context.ItemWareHouse.Where(x => x.ItemId == itemId && x.WhsCode == whsCode).Select(x => x.AvgPrice).FirstOrDefault();
            return price;
        }

        //InventoryReturn
        public List<InventoryReturnDto> GetBaseInventoryReturn(Expression<Func<InventoryReturn, bool>> condition)
        {
            var result = _context.InventoryReturn
                .Include(ci => ci.Detail)
                .Where(condition)
                .Join(_context.Seller, inv => inv.SellerId, s => s.SellerId, (inv, s) => new { InventoryReturn = inv, Seller = s })
                .Join(_context.SellerRegion, combined => combined.Seller.RegionId, sr => sr.RegionId, (combined, sr) => new { combined.InventoryReturn, combined.Seller, SellerRegion = sr })
                .Join(_context.WareHouse, combined => combined.InventoryReturn.WhsCode, wh => wh.WhsCode, (combined, wh) => new { combined.InventoryReturn, combined.Seller, combined.SellerRegion, WareHouse = wh })
                .Join(_context.User, combined => combined.InventoryReturn.CreatedBy, u => u.UserId, (combined, u) => new { combined.InventoryReturn, combined.Seller, combined.SellerRegion, combined.WareHouse, User = u })

                .Select(combined => new InventoryReturnDto
                {
                    Id = combined.InventoryReturn.Id,
                    DocDate = combined.InventoryReturn.DocDate,
                    SellerId = combined.InventoryReturn.SellerId,
                    RegionId = combined.InventoryReturn.RegionId,
                    WhsCode = combined.InventoryReturn.WhsCode,
                    CreatedDate = combined.InventoryReturn.CreatedDate,
                    CreatedBy = combined.InventoryReturn.CreatedBy,
                    Canceled = combined.InventoryReturn.Canceled,
                    Active = combined.InventoryReturn.Active,
                    SellerName = combined.Seller.SellerName,
                    RegionName = combined.SellerRegion.NameRegion,
                    WhsName = combined.WareHouse.WhsName,
                    CreatedByName = combined.User.Name,
                    Complete = combined.InventoryReturn.Complete,
                    Detail = combined.InventoryReturn.Detail
                })
                .ToList();
            return result;
        }


        public List<InventoryReturnDto> GetInventoryReturnByDate(DateTime fro, DateTime to)
        {
            return GetBaseInventoryReturn(x => x.DocDate.Date >= fro.Date && x.DocDate.Date <= to.Date);
        }

        public List<InventoryReturnDto> AddInventoryReturn(InventoryReturn request)
        {
            try
            {
                request.CreatedDate = DateTime.Now;
                request.Detail.ForEach(x => x.IdDetail = 0);
                _context.InventoryReturn.Add(request);
                _context.SaveChanges();
                return GetBaseInventoryReturn(x => x.DocDate.Date >= request.DocDate.Date && x.DocDate.Date <= request.DocDate.Date);
            }
            catch(Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                throw new Exception(mensaje);
            }
        }

        public List<InventoryReturnDto> UpdateInventoryReturn(InventoryReturn request)
        {
            try
            {
                // Buscar la entidad que se desea actualizar
                var existingInventoryReturn = _context.InventoryReturn.Include(ir => ir.Detail).FirstOrDefault(x => x.Id == request.Id);
                if (existingInventoryReturn != null)
                {
                    // Actualizar propiedades individuales
                    existingInventoryReturn.SellerId = request.SellerId;
                    existingInventoryReturn.RegionId = request.RegionId;
                    existingInventoryReturn.WhsCode = request.WhsCode;
                    existingInventoryReturn.DocDate = request.DocDate;
                    existingInventoryReturn.CreatedBy = request.CreatedBy;

                    // Manejo del detalle
                    if (request.Detail != null)
                    {

                        // Elimina detalles antiguos
                        _context.InventoryReturnDetail.RemoveRange(existingInventoryReturn.Detail);
                        existingInventoryReturn.Detail.Clear();

                        // Agrega los nuevos detalles
                        foreach (var detail in request.Detail)
                        {
                            detail.IdDetail = 0;
                            detail.IdReturn = request.Id;
                            existingInventoryReturn.Detail.Add(detail);
                        }
                    }
                    _context.InventoryReturnDetail.AddRange(existingInventoryReturn.Detail);
                    _context.InventoryReturn.Update(existingInventoryReturn);
                    int affectedRows = _context.SaveChanges();

                    if (affectedRows == 0)
                    {
                        throw new DbUpdateConcurrencyException("No rows were affected. The entity may have been modified or deleted.");
                    }
                }
                else
                {
                    throw new Exception("Entity not found.");
                }

                return GetBaseInventoryReturn(x => x.DocDate.Date >= request.DocDate.Date && x.DocDate.Date <= request.DocDate.Date);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // Manejo específico de excepciones de concurrencia
                var mensaje = "Concurrency conflict: " + ex.Message;
                throw new Exception(mensaje);
            }
            catch (Exception ex)
            {
                // Manejo de otras excepciones
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                throw new Exception(mensaje);
            }
        }


        public List<InventoryReturnDto> CompleteInventoryReturn(InventoryReturn request)
        {
            try
            {
                // Buscar la entidad que se desea actualizar
                var existingInventoryReturn = _context.InventoryReturn.Include(ir => ir.Detail).FirstOrDefault(x => x.Id == request.Id);
                if (existingInventoryReturn != null)
                {      
                    existingInventoryReturn.Complete = request.Complete;
                    _context.InventoryReturn.Update(existingInventoryReturn);
                    _context.SaveChanges();
                }
                return GetBaseInventoryReturn(x => x.DocDate.Date >= request.DocDate.Date && x.DocDate.Date <= request.DocDate.Date);
            }
            catch (Exception ex)
            {
                var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                throw new Exception(mensaje);
            }
        }
        public List<InventoryReturnDetailDTO> GetResumenReturn(DateTime date, int whsCode)
        {
            var exist = _context.InventoryReturn.Include(x => x.Detail)
                .Where(x => x.DocDate.Date == date.Date && x.WhsCode == whsCode)
                .FirstOrDefault();

            if (exist != null && exist.Complete)
            {
                throw new Exception("Para este vendedor ya se realizo la devolucion del dia seleccionado.");
            }

            // Ejecutar el procedimiento almacenado y mapear los resultados a InventoryReturnDetailDTO
            var resumenReturn = _context.Set<InventoryReturnDetailDTO>()
                .FromSqlRaw($@"
                    EXEC [dbo].[ObtenerResumenItemsDevolucion] 
                    @Fecha = '{date.ToString("yyyy-MM-dd")}',
                    @AlmacenCode = {whsCode}
                    ")
                .ToList();

            if (exist != null)
            {
                var existDetails = exist.Detail.ToDictionary(d => d.ItemId);
                foreach (var item in resumenReturn)
                {
                    if (existDetails.TryGetValue(item.ItemId, out var existingDetail))
                    {
                        item.QuantityReturn = existingDetail.QuantityReturn;
                    }
                    else
                    {
                        item.IdDetail = 0;
                    }
                }
            }

            return resumenReturn;
        }
    }
}
