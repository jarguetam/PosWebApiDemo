using Pos.WebApi.Features.Common.Dto;
using Pos.WebApi.Features.Common.Entities;
using Pos.WebApi.Infraestructure;
using System.Collections.Generic;
using System.Linq;

namespace Pos.WebApi.Features.Common
{
    public class BPJornalServices
    {
        private readonly PosDbContext _context;

        public BPJornalServices(PosDbContext context)
        {
            _context = context;
        }

        public void AddLineBPJournal(BPJornal itemJournal)
        {
            _context.BPJornal.Add(itemJournal);
            _context.SaveChanges();
        }

        public void AddLinesJournal(List<BPJornal> itemJournal)
        {
            _context.BPJornal.AddRange(itemJournal);
            _context.SaveChanges();
        }

        public List<BPJornalDto> GetJournal (int bpId, string type)
        {
            var jornal = _context.BPJornal.Where(x => x.BpId == bpId && x.BpType == type).OrderByDescending(x=> x.CreateDate).ToList();
            var userId = jornal.Select(x=> x.CreateBy ).ToList();
            var users = _context.User.Where(x => userId.Contains(x.UserId)).ToList();
            if (type.Equals("C"))
            {
                var bp = _context.Customer.SingleOrDefault(x => x.CustomerId == bpId);

                var orderedJornal = jornal
                    .Where(j => j.BpId == bpId) // Filtrar las transacciones del cliente específico
                    .OrderBy(j => j.CreateDate) // Ordenar por fecha para calcular el balance en orden
                    .ToList();

                decimal balance = 0;
                var resultList = new List<BPJornalDto>();

                foreach (var journalEntry in orderedJornal)
                {
                    balance += journalEntry.TransValue;

                    var result = new BPJornalDto
                    {
                        BpId = journalEntry.BpId,
                        BpType = journalEntry.BpType,
                        BPJournalId = journalEntry.BPJournalId,
                        BusinnesPartnersCode = bp.CustomerCode,
                        BusinnesName = bp.CustomerName,
                        DocId = journalEntry.DocId,
                        TransValue = journalEntry.TransValue,
                        DocumentReferent = journalEntry.DocumentReferent,
                        Documents = journalEntry.Documents,
                        CreateDate = journalEntry.CreateDate,
                        CreateBy = journalEntry.CreateBy,
                        CreateByName = users.SingleOrDefault(u => u.UserId == journalEntry.CreateBy)?.Name,
                        Balance = balance // El balance se actualiza en cada iteración
                    };

                    resultList.Add(result);
                }

                return resultList.OrderByDescending(x=> x.CreateDate).ToList();



            }
            else
            {
                var bp = _context.Supplier.SingleOrDefault(x => x.SupplierId == bpId);

                var orderedJornal = jornal
                    .Where(j => j.BpId == bpId) // Filtrar las transacciones del cliente específico
                    .OrderBy(j => j.CreateDate) // Ordenar por fecha para calcular el balance en orden
                    .ToList();

                decimal balance = 0;
                var resultList = new List<BPJornalDto>();

                foreach (var journalEntry in orderedJornal)
                {
                    balance += journalEntry.TransValue;

                    var result = new BPJornalDto
                    {
                        BpId = journalEntry.BpId,
                        BpType = journalEntry.BpType,
                        BPJournalId = journalEntry.BPJournalId,
                        BusinnesPartnersCode = bp.SupplierCode,
                        BusinnesName = bp.SupplierName,
                        DocId = journalEntry.DocId,
                        TransValue = journalEntry.TransValue,
                        DocumentReferent = journalEntry.DocumentReferent,
                        Documents = journalEntry.Documents,
                        CreateDate = journalEntry.CreateDate,
                        CreateBy = journalEntry.CreateBy,
                        CreateByName = users.SingleOrDefault(u => u.UserId == journalEntry.CreateBy)?.Name,
                        Balance = balance // El balance se actualiza en cada iteración
                    };

                    resultList.Add(result);
                }

                return resultList.OrderByDescending(x => x.CreateDate).ToList();
            }
            
        }
    }
}
