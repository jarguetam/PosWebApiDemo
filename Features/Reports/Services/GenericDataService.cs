using Pos.WebApi.Infraestructure;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using Pos.WebApi.Features.Reports.Dto;

namespace Pos.WebApi.Features.Reports.Services
{
    public class GenericDataService
    {
        private readonly PosDbContext _context;

        public GenericDataService(PosDbContext context)
        {
            _context = context;
        }

        public List<ReportView> GetReports()
        {
            return _context.ReportView.ToList();
        }

        public async Task<List<Dictionary<string, object>>> ExecuteQueryAsync(string sqlQuery, object parameters = null)
        {
            var result = new List<Dictionary<string, object>>();

            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sqlQuery;
                command.CommandType = CommandType.Text;

                if (parameters != null)
                {
                    foreach (var prop in parameters.GetType().GetProperties())
                    {
                        var param = command.CreateParameter();
                        param.ParameterName = $"@{prop.Name}";
                        param.Value = prop.GetValue(parameters) ?? DBNull.Value;
                        command.Parameters.Add(param);
                    }
                }

                await _context.Database.OpenConnectionAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var row = new Dictionary<string, object>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            row[reader.GetName(i)] = reader.GetValue(i);
                        }
                        result.Add(row);
                    }
                }
            }

            return result;
        }

        public async Task<List<Dictionary<string, object>>> ExecuteStoredProcedureAsync(string procedureName, object parameters = null)
        {
            var result = new List<Dictionary<string, object>>();

            var connection = _context.Database.GetDbConnection();
            try
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = procedureName;
                    command.CommandType = CommandType.StoredProcedure;

                    if (parameters != null)
                    {
                        foreach (var prop in parameters.GetType().GetProperties())
                        {
                            var param = command.CreateParameter();
                            param.ParameterName = $"@{prop.Name}";
                            param.Value = prop.GetValue(parameters) ?? DBNull.Value;
                            command.Parameters.Add(param);
                        }
                    }

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var row = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                            }
                            result.Add(row);
                        }
                    }
                }
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    await connection.CloseAsync();
            }

            return result;
        }
    }
}
