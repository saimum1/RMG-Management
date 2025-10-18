using DotNetWbapi.Data;
using DotNetWbapi.Dtos;
using DotNetWbapi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetWbapi.Services
{

    // Services/TruckService.cs
    public class TruckService
    {
        private readonly AppDBContext _context;
        private readonly ILogger<TruckService> _logger;

        public TruckService(AppDBContext context, ILogger<TruckService> logger)
        {
            _context = context;
            _logger = logger;
        }


        // Services/TruckService.cs

        public async Task<List<TruckResponseDto>> GetTrucksAsync()
        {
            return await _context.Trucks
                .Select(t => new TruckResponseDto
                {
                    Id = t.Id,
                    TruckNumber = t.TruckNumber,
                    TruckLicense = t.TruckLicense,
                    TransportCompanyName = t.TransportCompanyName,
                    DriverName = t.DriverName,
                    DriverLicense = t.DriverLicense,
                    DriverPhone = t.DriverPhone,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt
                })
                .ToListAsync();
        }

        // Services/TruckService.cs
        public async Task<TruckResponseDto?> GetTruckAsync(Guid id)
        {
            var truck = await _context.Trucks.FindAsync(id);

            if (truck == null)
            {
                _logger.LogWarning("Truck with Id {Id} not found", id);
                return null;
            }

            return new TruckResponseDto
            {
                Id = truck.Id,
                TruckNumber = truck.TruckNumber,
                TruckLicense = truck.TruckLicense,
                TransportCompanyName = truck.TransportCompanyName,
                DriverName = truck.DriverName,
                DriverLicense = truck.DriverLicense,
                DriverPhone = truck.DriverPhone,
                CreatedAt = truck.CreatedAt,
                UpdatedAt = truck.UpdatedAt
            };
        }

        public async Task<TruckResponseDto> CreateTruckAsync(TruckDto dto)
        {
            _logger.LogInformation("Creating truck with TruckNumber: {TruckNumber}", dto.TruckNumber);

            var existingTruck = await _context.Trucks
                .AnyAsync(t => t.TruckNumber == dto.TruckNumber && t.Id != dto.Id);
            if (existingTruck)
            {
                _logger.LogWarning("Truck with TruckNumber {TruckNumber} already exists", dto.TruckNumber);
                throw new InvalidOperationException("A truck with this Truck Number already exists.");
            }

            var truck = new Truck
            {
                Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
                TruckNumber = dto.TruckNumber,
                TruckLicense = dto.TruckLicense,
                TransportCompanyName = dto.TransportCompanyName,
                DriverName = dto.DriverName,
                DriverLicense = dto.DriverLicense,
                DriverPhone = dto.DriverPhone,
                UpdatedAt = DateTime.UtcNow
            };

            if (dto.Id == Guid.Empty) 
            {
                truck.CreatedAt = DateTime.UtcNow;
                _context.Trucks.Add(truck);
            }
            else 
            {
                _context.Trucks.Update(truck);
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Created/Updated truck with Id: {Id}", truck.Id);
            return new TruckResponseDto
            {
                Id = truck.Id,
                TruckNumber = truck.TruckNumber,
                TruckLicense = truck.TruckLicense,
                TransportCompanyName = truck.TransportCompanyName,
                DriverName = truck.DriverName,
                DriverLicense = truck.DriverLicense,
                DriverPhone = truck.DriverPhone,
                CreatedAt = truck.CreatedAt,
                UpdatedAt = truck.UpdatedAt
            };
        }

        public async Task<List<TruckResponseDto>> CreateTrucksAsync(TruckListDto dto)
        {
            _logger.LogInformation("Creating {Count} trucks", dto.Trucks.Count);

            var result = new List<TruckResponseDto>();
            
            foreach (var truckDto in dto.Trucks)
            {
                try
                {
                    var existingTruck = await _context.Trucks
                        .AnyAsync(t => t.TruckNumber == truckDto.TruckNumber && t.Id != truckDto.Id);
                    
                    if (existingTruck)
                    {
                        _logger.LogWarning("Truck with TruckNumber {TruckNumber} already exists, skipping", truckDto.TruckNumber);
                        continue;
                    }

                    var truck = new Truck
                    {
                        Id = truckDto.Id == Guid.Empty ? Guid.NewGuid() : truckDto.Id,
                        TruckNumber = truckDto.TruckNumber,
                        TruckLicense = truckDto.TruckLicense,
                        TransportCompanyName = truckDto.TransportCompanyName,
                        DriverName = truckDto.DriverName,
                        DriverLicense = truckDto.DriverLicense,
                        DriverPhone = truckDto.DriverPhone,
                        CreatedAt = truckDto.Id == Guid.Empty ? DateTime.UtcNow : DateTime.MinValue,
                        UpdatedAt = DateTime.UtcNow
                    };

                    if (truckDto.Id == Guid.Empty) 
                    {
                        _context.Trucks.Add(truck);
                    }
                    else 
                    {
                        _context.Trucks.Update(truck);
                    }

                    result.Add(new TruckResponseDto
                    {
                        Id = truck.Id,
                        TruckNumber = truck.TruckNumber,
                        TruckLicense = truck.TruckLicense,
                        TransportCompanyName = truck.TransportCompanyName,
                        DriverName = truck.DriverName,
                        DriverLicense = truck.DriverLicense,
                        DriverPhone = truck.DriverPhone,
                        CreatedAt = truck.CreatedAt,
                        UpdatedAt = truck.UpdatedAt
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating truck with TruckNumber {TruckNumber}", truckDto.TruckNumber);
                }
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Created {Count} trucks successfully", result.Count);
            return result;
        }

        public async Task<bool> UpdateTruckAsync(Guid id, TruckDto dto)
        {
            if (id != dto.Id)
            {
                _logger.LogWarning("ID mismatch in update request for truck");
                return false;
            }

            var truck = await _context.Trucks.FindAsync(id);
            if (truck == null)
            {
                _logger.LogWarning("Truck with Id {Id} not found", id);
                return false;
            }

            // Check if another truck with the same truck number already exists
            var existingTruck = await _context.Trucks
                .AnyAsync(t => t.TruckNumber == dto.TruckNumber && t.Id != id);
            if (existingTruck)
            {
                _logger.LogWarning("Truck with TruckNumber {TruckNumber} already exists", dto.TruckNumber);
                throw new InvalidOperationException("A truck with this Truck Number already exists.");
            }

            truck.TruckNumber = dto.TruckNumber;
            truck.TruckLicense = dto.TruckLicense;
            truck.TransportCompanyName = dto.TransportCompanyName;
            truck.DriverName = dto.DriverName;
            truck.DriverLicense = dto.DriverLicense;
            truck.DriverPhone = dto.DriverPhone;
            truck.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Updated truck with Id: {Id}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating truck with Id: {Id}", id);
                return false;
            }
        }

        public async Task<bool> DeleteTruckAsync(Guid id)
        {
            var truck = await _context.Trucks.FindAsync(id);
            if (truck == null)
            {
                _logger.LogWarning("Truck with Id {Id} not found", id);
                return false;
            }

            _context.Trucks.Remove(truck);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Deleted truck with Id: {Id}", id);
            return true;
        }
    }
}