

// Services/GatepassService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DotNetWbapi.Models;
using DotNetWbapi.Dtos;
using Microsoft.Extensions.Logging;
using DotNetWbapi.Data;
using System.Data.SqlClient;

namespace DotNetWbapi.Services
{
    public class GatepassService
    {
        private readonly AppDBContext _context;
        private readonly DbHelper _db;

        private readonly ILogger<GatepassService> _logger;

        public GatepassService(AppDBContext context, ILogger<GatepassService> logger ,IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            var useMySql = bool.Parse(Environment.GetEnvironmentVariable("USE_SERVER_SQL") ?? "false");
            var connString = useMySql
                ? Environment.GetEnvironmentVariable("SERVER_DB_CONNECTION") ?? ""
                : $"Server={Environment.GetEnvironmentVariable("LOCAL_DB_SERVER")};Database={Environment.GetEnvironmentVariable("LOCAL_DB_NAME")};User Id={Environment.GetEnvironmentVariable("LOCAL_DB_USER")};Password={Environment.GetEnvironmentVariable("LOCAL_DB_PASSWORD")};TrustServerCertificate=True;";

            _db = new DbHelper(connString, useMySql);
        }

        public async Task<GatepassHeaderDto> CreateGatepassAsync(GatepassHeaderDto dto)
        {
            _logger.LogInformation("Creating gatepass with GatePassNo: {GatePassNo}", dto.GatePassNo);

            var existingGatepass = await _context.GatepassHeaders
                .AnyAsync(h => h.GatePassNo == dto.GatePassNo && h.Id != dto.Id);
            if (existingGatepass)
            {
                _logger.LogWarning("Gatepass with GatePassNo {GatePassNo} already exists", dto.GatePassNo);
                throw new InvalidOperationException("A gatepass with this Gate Pass No. already exists.");
            }

            var header = new GatepassHeader
            {
                Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
                GatePassNo = dto.GatePassNo,
                Date = dto.Date,
                CompanyName = dto.CompanyName,
                Address = dto.Address,
                IssuedTo = dto.IssuedTo,
                TruckNo = dto.TruckNo,
                DriverName = dto.DriverName,
                TransportCompany = dto.TransportCompany,
                DriverPhone = dto.DriverPhone,
                DriverLicense = dto.DriverLicense,
                Remarks = dto.Remarks
            };

            if (dto.Id == Guid.Empty)
            {
                _context.GatepassHeaders.Add(header);
            }
            else
            {
                _context.GatepassHeaders.Update(header);
            }

            await _context.SaveChangesAsync();

            dto.Id = header.Id;
            _logger.LogInformation("Created/Updated gatepass with Id: {Id}", header.Id);
            return dto;
        }

        public async Task<GatepassHeaderDto?> GetGatepassByIdAsync(Guid id)
        {
            var gatepass = await _context.GatepassHeaders.FindAsync(id);
            if (gatepass == null)
            {
                return null;
            }

            return new GatepassHeaderDto
            {
                Id = gatepass.Id,
                GatePassNo = gatepass.GatePassNo,
                Date = gatepass.Date,
                CompanyName = gatepass.CompanyName,
                Address = gatepass.Address,
                IssuedTo = gatepass.IssuedTo,
                TruckNo = gatepass.TruckNo,
                DriverName = gatepass.DriverName,
                TransportCompany = gatepass.TransportCompany,
                DriverPhone = gatepass.DriverPhone,
                DriverLicense = gatepass.DriverLicense,
                Remarks = gatepass.Remarks
            };
        }

        public async Task<List<GatepassHeaderDto>> GetAllGatepassesAsync()
        {
            return await _context.GatepassHeaders
                .Select(g => new GatepassHeaderDto
                {
                    Id = g.Id,
                    GatePassNo = g.GatePassNo,
                    Date = g.Date,
                    CompanyName = g.CompanyName,
                    Address = g.Address,
                    IssuedTo = g.IssuedTo,
                    TruckNo = g.TruckNo,
                    DriverName = g.DriverName,
                    TransportCompany = g.TransportCompany,
                    DriverPhone = g.DriverPhone,
                    DriverLicense = g.DriverLicense,
                    Remarks = g.Remarks
                })
                .ToListAsync();
        }

        public async Task<bool> DeleteGatepassAsync(Guid id)
        {
            var gatepass = await _context.GatepassHeaders.FindAsync(id);
            if (gatepass == null)
            {
                return false;
            }

            _context.GatepassHeaders.Remove(gatepass);
            await _context.SaveChangesAsync();
            return true;
        }


    public async Task<bool> SyncGatepassAsync(List<Guid> ids)
    {
        if (ids == null || ids.Count == 0)
            return false;

        var currentDate = DateTime.Now.ToString("yyyy-MM-dd");
        var currentTime = DateTime.Now.ToString("HH:mm");

        // Build parameter placeholders: @id0, @id1, ...
        var inParams = string.Join(", ", ids.Select((_, i) => $"@id{i}"));

        // Build the query
        var sql = $@"
            UPDATE DeliveryChallanHeaderCreation
            SET OutDate = @OutDate,
                OutTime = @OutTime
            WHERE Id IN ({inParams})";

        // Build parameters array
        var parameters = ids.Select((id, i) => ($"@id{i}", (object)id)).ToList();
        parameters.Add(("@OutDate", currentDate));
        parameters.Add(("@OutTime", currentTime));

        try
        {
            int rows = await _db.ExecuteAsync(sql, parameters.ToArray());
            return rows > 0;
        }
        catch (Exception ex)
        {
            // Log error
            Console.WriteLine($"Error syncing gatepasses: {ex}");
            return false;
        }
    }



    }
}