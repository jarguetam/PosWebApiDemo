using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Pos.WebApi.Features.Common.Dto;
using Pos.WebApi.Features.Common.Entities;
using Pos.WebApi.Features.Reports.Services;
using Pos.WebApi.Infraestructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Pos.WebApi.Features.Common
{
    public class BPJornalServices
    {
        private readonly PosDbContext _context;
        private readonly GenericDataService _genericDataService;
        private readonly IConfiguration _config;

        public BPJornalServices(PosDbContext context, GenericDataService genericDataService, IConfiguration config)
        {
            _context = context;
            _genericDataService = genericDataService;
            _config = config;

        }

        public void AddLineBPJournal(BPJornal itemJournal)
        {
            _context.BPJornal.Add(itemJournal);
            _context.SaveChanges();
        }

        public async Task AddLineBPJournalAsync(BPJornal itemJournal)
        {
            await _context.BPJornal.AddAsync(itemJournal);
            await _context.SaveChangesAsync();
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


        public async Task<List<Dictionary<string, object>>> GetCustomerJournal(int customerId, DateTime from, DateTime to)
        {
            string conec = _config["connectionStrings:dbpos"];
            using (IDbConnection db = new SqlConnection(conec))
            {
                var parameters = new DynamicParameters();
                parameters.Add("bpId", customerId, DbType.Int32);
                parameters.Add("startDate", from, DbType.DateTime);
                parameters.Add("endDate", to, DbType.DateTime);

                var result = await db.QueryAsync("sp_GetCustomerJournal", parameters, commandType: CommandType.StoredProcedure);

                var resultList = new List<Dictionary<string, object>>();
                foreach (var row in result)
                {
                    var dict = new Dictionary<string, object>();
                    foreach (var property in row)
                    {
                        dict.Add(property.Key, property.Value);
                    }
                    resultList.Add(dict);
                }

                return resultList;
            }
        }



    }
}
