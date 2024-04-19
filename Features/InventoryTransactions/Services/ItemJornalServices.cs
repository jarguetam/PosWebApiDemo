using Pos.WebApi.Features.InventoryTransactions.Dto;
using Pos.WebApi.Features.InventoryTransactions.Entities;
using Pos.WebApi.Infraestructure;
using System.Collections.Generic;
using System.Linq;

namespace Pos.WebApi.Features.InventoryTransactions.Services
{
    public class ItemJournalServices
    {
        private readonly PosDbContext _context;

        public ItemJournalServices(PosDbContext context)
        {
            _context = context;
        }

        public void AddLineJournal(ItemJournal itemJournal)
        {
            _context.ItemJournal.Add(itemJournal);
            _context.SaveChanges();
        }

        public void AddLinesJournal(List<ItemJournal> itemJournal)
        {
            _context.ItemJournal.AddRange(itemJournal);
            _context.SaveChanges();
        }

        public List<ItemJournalDto> GetJornalItems(int itemId)
        {
            var journal = _context.ItemJournal.Where(x => x.ItemId == itemId).ToList();
            var items = _context.Item.Where(x => x.ItemId == itemId).ToList();
            var userId = journal.Select(x => x.CreateBy).Distinct().ToList();
            var users = _context.User.Where(x => userId.Contains(x.UserId)).ToList();
            var whsId = journal.Select(x => x.WhsCode).Distinct().ToList();
            var whs = _context.WareHouse.Where(x=> whsId.Contains(x.WhsCode)).ToList();
            var result = (from j in journal
                          join i in items on j.ItemId equals i.ItemId
                          join u in users on j.CreateBy equals u.UserId
                          join w in whs on j.WhsCode equals w.WhsCode
                          orderby j.CreateDate
                          select new ItemJournalDto
                          {
                              ItemJournalId = j.ItemJournalId,
                              ItemId = j.ItemId,
                              ItemCode = i.ItemCode,
                              ItemName = i.ItemName,
                              WhsCode = j.WhsCode,
                              WhsName = w.WhsName,
                              Quantity = j.Quantity,
                              Price = j.Price,
                              TransValue = j.TransValue,
                              Documents = j.Documents,
                              DocumentReferent = j.DocumentReferent,
                              CreateBy = j.CreateBy,
                              CreateByName = u.Name,
                              CreateDate = j.CreateDate
                          }
                          ).ToList();

            return result;
        }
    }
}
