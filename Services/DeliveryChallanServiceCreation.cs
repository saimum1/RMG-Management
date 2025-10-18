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
    public class DeliveryChallanServiceCreation
    {
        private readonly AppDBContext _context;
        private readonly ILogger<DeliveryChallanServiceCreation> _logger;

        public DeliveryChallanServiceCreation(AppDBContext context, ILogger<DeliveryChallanServiceCreation> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<DeliveryChallanHeaderCreationDto>> GetDeliveryChallansAsync()
        {
            return await _context.DeliveryChallanHeaderCreation
            
                .Select(h => new DeliveryChallanHeaderCreationDto
                {
                    Id = h.Id,
                    ChallanNo = h.ChallanNo,
                    // ExistingTruckNo = h.ExistingTruckNo,
                    ExistingTruckNo = h.ExistingTruckNo ?? "Default Value", 
                    HireTruckNo = h.HireTruckNo,
                    InDate = h.InDate,
                    InTime = h.InTime,
                    To = h.To,
                    DriverName = h.DriverName,
                    OutDate = h.OutDate,
                    OutTime = h.OutTime,
                    Address = h.Address,
                    LicenseNo = h.LicenseNo,
                    TruckCBM = h.TruckCBM,
                    DepotName = h.DepotName,
                    MobileNo = h.MobileNo,
                    TransportCompany = h.TransportCompany,
                    LockNo = h.LockNo,
                    RentedAmount = h.RentedAmount,

                    remarks = h.remarks,
                    descriptions = h.descriptions,
                    agdl = h.agdl,
                    unit = h.unit,
                    quantity = h.quantity,



                    // CBMItems = h.CBMDetails.Select(d => new DeliveryChallanDetailCreationDto
                    // {
                    //     Id = d.Id,
                    //     CompanyWiseCBM = d.CompanyWiseCBM,
                    //     CBM = d.CBM,
                    //     RentAmount = d.RentAmount
                    // }).ToList()
                })
                .ToListAsync();
        }

        public async Task<DeliveryChallanHeaderCreationDto> CreateDeliveryChallanAsync(DeliveryChallanHeaderCreationDto dto)
        {
            _logger.LogInformation("Creating delivery challan with ChallanNo: {ChallanNo}", dto);
            // Console.WriteLine("Creating delivery challan with ChallanNo: {ChallanNo}", dto); 
            var existingChallan = await _context.DeliveryChallanHeaderCreation
                .AnyAsync(h => h.ChallanNo == dto.ChallanNo && h.Id != dto.Id);
            if (existingChallan)
            {
                _logger.LogWarning("Delivery challan with ChallanNo {ChallanNo} already exists", dto.ChallanNo);
                throw new InvalidOperationException("A delivery challan with this Challan No. already exists.");
            }
 
            var header = new DeliveryChallanHeaderCreation
            {
                Id = dto.Id == Guid.Empty ? Guid.NewGuid() : dto.Id,
                ChallanNo = dto.ChallanNo,
                ExistingTruckNo = dto.ExistingTruckNo,
                HireTruckNo = dto.HireTruckNo,
                InDate = dto.InDate,
                InTime = dto.InTime,
                To = dto.To,
                DriverName = dto.DriverName,
                OutDate = dto.OutDate,

                OutTime = dto.OutTime,
                Address = dto.Address,
                LicenseNo = dto.LicenseNo,
                TruckCBM = dto.TruckCBM,
                DepotName = dto.DepotName,
                MobileNo = dto.MobileNo,
                TransportCompany = dto.TransportCompany,
                LockNo = dto.LockNo,
                RentedAmount = dto.RentedAmount,
                    remarks = dto.remarks,
                    descriptions = dto.descriptions,
                    agdl = dto.agdl,
                    unit = dto.unit,
                    quantity = dto.quantity,

            };

            if (dto.Id == Guid.Empty) _context.DeliveryChallanHeaderCreation.Add(header);
            else _context.DeliveryChallanHeaderCreation.Update(header);

            // var existingDetails = await _context.DeliveryChallanDetailCreation
            //     .Where(d => d.DeliveryChallanHeaderCreationId == header.Id).ToListAsync();
            // _context.DeliveryChallanDetailCreation.RemoveRange(existingDetails);

            // foreach (var detailDto in dto.CBMItems)
            // {
            //     var detail = new DeliveryChallanDetailCreation
            //     {
            //         Id = detailDto.Id == Guid.Empty ? Guid.NewGuid() : detailDto.Id,
            //         DeliveryChallanHeaderCreationId = header.Id,
            //         CompanyWiseCBM = detailDto.CompanyWiseCBM,
            //         CBM = detailDto.CBM,
            //         RentAmount = detailDto.RentAmount
            //     };
            //     _context.DeliveryChallanDetailCreation.Add(detail);
            // }

            await _context.SaveChangesAsync();

            dto.Id = header.Id;
            _logger.LogInformation("Created/Updated delivery challan with Id: {Id}", header.Id);
            return dto;
        }

        public async Task<DeliveryChallanHeaderCreationDto?> GetDeliveryChallanAsync(Guid id)
        {
            var header = await _context.DeliveryChallanHeaderCreation
                // .Include(h => h.CBMDetails)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (header == null)
            {
                _logger.LogWarning("Delivery challan with Id {Id} not found", id);
                return null;
            }

            return new DeliveryChallanHeaderCreationDto
            {
                Id = header.Id,
                ChallanNo = header.ChallanNo,
                ExistingTruckNo = header.ExistingTruckNo ?? string.Empty,
                HireTruckNo = header.HireTruckNo,
                InDate = header.InDate,
                InTime = header.InTime,
                To = header.To,
                DriverName = header.DriverName,
                OutDate = header.OutDate,
                OutTime = header.OutTime,
                Address = header.Address,
                LicenseNo = header.LicenseNo,
                TruckCBM = header.TruckCBM,
                DepotName = header.DepotName,
                MobileNo = header.MobileNo,
                TransportCompany = header.TransportCompany,
                LockNo = header.LockNo,
                RentedAmount = header.RentedAmount,
remarks = header.remarks,
                    descriptions = header.descriptions,
                    agdl = header.agdl,
                    unit = header.unit,
                    quantity = header.quantity,
                // CBMItems = header.CBMDetails.Select(d => new DeliveryChallanDetailCreationDto
                // {
                //     Id = d.Id,
                //     CompanyWiseCBM = d.CompanyWiseCBM,
                //     CBM = d.CBM,
                //     RentAmount = d.RentAmount
                // }).ToList()
            };
        }

        public async Task<bool> DeleteDeliveryChallanAsync(Guid id)
        {
            var header = await _context.DeliveryChallanHeaderCreation
                // .Include(h => h.CBMDetails)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (header == null)
            {
                _logger.LogWarning("Delivery challan with Id {Id} not found", id);
                return false;
            }

            // _context.DeliveryChallanDetailCreation.RemoveRange(header.CBMDetails);
            _context.DeliveryChallanHeaderCreation.Remove(header);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Deleted delivery challan with Id: {Id}", id);
            return true;
        }




        
    }
}