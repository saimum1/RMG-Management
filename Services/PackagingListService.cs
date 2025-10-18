using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DotNetWbapi.Models;
using DotNetWbapi.Dtos;
using Microsoft.Extensions.Logging;
using DotNetWbapi.Data;

namespace DotNetWbapi.Services
{
    public class PackagingListService
    {
        private readonly AppDBContext _context;
        private readonly ILogger<PackagingListService> _logger;
        private readonly DeclarationSettingService _declarationSettingService;

        public PackagingListService(
            AppDBContext context,
            ILogger<PackagingListService> logger,
            DeclarationSettingService declarationSettingService)
        {
            _context = context;
            _logger = logger;
            _declarationSettingService = declarationSettingService;
        }

        // GET by ID
        public async Task<PackagingListDto?> GetPackagingListByIdAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Fetching packaging list with ID: {Id}", id);
                var packagingList = await _context.PackagingLists.FindAsync(id);

                if (packagingList == null)
                {
                    _logger.LogWarning("Packaging list with ID: {Id} not found", id);
                    return null;
                }

                return MapToDto(packagingList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching packaging list with ID: {Id}", id);
                throw;
            }
        }

        // GET ALL
        public async Task<List<PackagingListDto>> GetAllPackagingListsAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all packaging lists");
                var packagingLists = await _context.PackagingLists
                    .OrderByDescending(pl => pl.CreatedAt)
                    .ToListAsync();

                return packagingLists.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all packaging lists");
                throw;
            }
        }

        // CREATE
        public async Task<PackagingListDto> CreatePackagingListAsync(CreatePackagingListDto dto)
        {
            try
            {
                _logger.LogInformation("Creating packaging list for company: {Company}, buyer: {Buyer}", dto.Company, dto.Buyer);

                var packagingList = new PackagingList
                {
                    Id = Guid.NewGuid(),
                    Company = dto.Company,
                    Buyer = dto.Buyer,
                    Style = dto.Style,
                    PurchaseOrder = dto.PurchaseOrder,
                    StyleBuyer = dto.StyleBuyer,
                    ShippedBy = dto.ShippedBy,
                    ShortDescription = dto.ShortDescription,
                    Size = dto.Size,
                    Color = dto.Color,
                    Brand = dto.Brand,
                    Packing = dto.Packing,
                    PcsPerSet = dto.PcsPerSet,
                    ProductNo = dto.ProductNo,
                    CountryOfOrigin = dto.CountryOfOrigin,
                    PortOfLoading = dto.PortOfLoading,
                    CountryOfDestination = dto.CountryOfDestination,
                    PortOfDischarge = dto.PortOfDischarge,
                    Incoterm = dto.Incoterm,
                    OrmsNo = dto.OrmsNo,
                    LmkgNo = dto.LmkgNo,
                    OrmsStyleNo = dto.OrmsStyleNo,
                    Status = dto.Status,

                    // Buyer Specific Fields
                    ItemNo = dto.ItemNo,
                    Pod = dto.Pod,
                    Boi = dto.Boi,
                    Wwwk = dto.Wwwk,
                    NoOfColor = dto.NoOfColor,
                    KeyCode = dto.KeyCode,
                    SupplierCode = dto.SupplierCode,

                    // Additional Fields
                    Cartons = dto.Cartons,
                    OrderQtyPac = dto.OrderQtyPac,
                    OrderQtyPcs = dto.OrderQtyPcs,
                    ShipQtyPac = dto.ShipQtyPac,
                    ShipQtyPcs = dto.ShipQtyPcs,
                    Destination = dto.Destination,

                    CreatedAt = DateTime.UtcNow
                };

                // Map packing details and declaration answers
                packagingList.SetPackingDetails(MapPackingDetails(dto.PackingDetails));
                packagingList.SetDeclarationAnswers(MapDeclarationAnswers(dto.DeclarationAnswers));

                _context.PackagingLists.Add(packagingList);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully created packaging list with ID: {Id}", packagingList.Id);
                return MapToDto(packagingList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating packaging list for company: {Company}, buyer: {Buyer}", dto.Company, dto.Buyer);
                throw;
            }
        }

        // UPDATE
        public async Task<PackagingListDto?> UpdatePackagingListAsync(Guid id, UpdatePackagingListDto dto)
        {
            try
            {
                _logger.LogInformation("Updating packaging list with ID: {Id}", id);

                var packagingList = await _context.PackagingLists.FindAsync(id);
                if (packagingList == null)
                {
                    _logger.LogWarning("Packaging list with ID: {Id} not found for update", id);
                    return null;
                }

                // Update basic properties
                packagingList.Company = dto.Company;
                packagingList.Buyer = dto.Buyer;
                packagingList.Style = dto.Style;
                packagingList.PurchaseOrder = dto.PurchaseOrder;
                packagingList.StyleBuyer = dto.StyleBuyer;
                packagingList.ShippedBy = dto.ShippedBy;
                packagingList.ShortDescription = dto.ShortDescription;
                packagingList.Size = dto.Size;
                packagingList.Color = dto.Color;
                packagingList.Brand = dto.Brand;
                packagingList.Packing = dto.Packing;
                packagingList.PcsPerSet = dto.PcsPerSet;
                packagingList.ProductNo = dto.ProductNo;
                packagingList.CountryOfOrigin = dto.CountryOfOrigin;
                packagingList.PortOfLoading = dto.PortOfLoading;
                packagingList.CountryOfDestination = dto.CountryOfDestination;
                packagingList.PortOfDischarge = dto.PortOfDischarge;
                packagingList.Incoterm = dto.Incoterm;
                packagingList.OrmsNo = dto.OrmsNo;
                packagingList.LmkgNo = dto.LmkgNo;
                packagingList.OrmsStyleNo = dto.OrmsStyleNo;
                packagingList.Status = dto.Status;

                // Update buyer specific fields
                packagingList.ItemNo = dto.ItemNo;
                packagingList.Pod = dto.Pod;
                packagingList.Boi = dto.Boi;
                packagingList.Wwwk = dto.Wwwk;
                packagingList.NoOfColor = dto.NoOfColor;
                packagingList.KeyCode = dto.KeyCode;
                packagingList.SupplierCode = dto.SupplierCode;

                // Update additional fields
                packagingList.Cartons = dto.Cartons;
                packagingList.OrderQtyPac = dto.OrderQtyPac;
                packagingList.OrderQtyPcs = dto.OrderQtyPcs;
                packagingList.ShipQtyPac = dto.ShipQtyPac;
                packagingList.ShipQtyPcs = dto.ShipQtyPcs;
                packagingList.Destination = dto.Destination;

                // Update packing details and declaration answers
                packagingList.SetPackingDetails(MapPackingDetails(dto.PackingDetails));
                packagingList.SetDeclarationAnswers(MapDeclarationAnswers(dto.DeclarationAnswers));

                packagingList.UpdatedAt = DateTime.UtcNow;

                _context.PackagingLists.Update(packagingList);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully updated packaging list with ID: {Id}", id);
                return MapToDto(packagingList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating packaging list with ID: {Id}", id);
                throw;
            }
        }

        // DELETE
        public async Task<bool> DeletePackagingListAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting packaging list with ID: {Id}", id);

                var packagingList = await _context.PackagingLists.FindAsync(id);
                if (packagingList == null)
                {
                    _logger.LogWarning("Packaging list with ID: {Id} not found for deletion", id);
                    return false;
                }

                _context.PackagingLists.Remove(packagingList);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted packaging list with ID: {Id}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting packaging list with ID: {Id}", id);
                throw;
            }
        }

        // GET Declaration Questions
        public async Task<List<DeclarationQuestionDto>> GetDeclarationQuestionsAsync()
        {
            try
            {
                _logger.LogInformation("Fetching declaration questions");
                var settings = await _declarationSettingService.GetAllDeclarationSettingsAsync();
                var companySettings = settings.FirstOrDefault(s => s.CompanyName == "Next Source Ltd");

                if (companySettings != null && companySettings.Questions.Any())
                {
                    return companySettings.Questions.Select((q, index) => new DeclarationQuestionDto
                    {
                        QuestionNumber = index + 1,
                        QuestionText = q
                    }).ToList();
                }

                // Return default questions if none found for the company
                return new List<DeclarationQuestionDto>
                {
                    new DeclarationQuestionDto { QuestionNumber = 1, QuestionText = "Have unacceptable packaging materials or bamboo products been used as packaging or dunnage in the consignment covered by this document?" },
                    new DeclarationQuestionDto { QuestionNumber = 2, QuestionText = "Has solid timber packaging/dunnage been used in consignments covered by this document?" },
                    new DeclarationQuestionDto { QuestionNumber = 3, QuestionText = "All timber packaging/dunnage used in the consignment has been (Please indicate below) Treated in compliance with Department of Agriculture and Water Resources treatment requirements" },
                    new DeclarationQuestionDto { QuestionNumber = 4, QuestionText = "statement to be removed from document when not relevant" },
                    new DeclarationQuestionDto { QuestionNumber = 5, QuestionText = "Issued Date" }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving declaration questions");

                // Return default questions in case of error
                return new List<DeclarationQuestionDto>
                {
                    new DeclarationQuestionDto { QuestionNumber = 1, QuestionText = "Have unacceptable packaging materials or bamboo products been used as packaging or dunnage in the consignment covered by this document?" },
                    new DeclarationQuestionDto { QuestionNumber = 2, QuestionText = "Has solid timber packaging/dunnage been used in consignments covered by this document?" },
                    new DeclarationQuestionDto { QuestionNumber = 3, QuestionText = "All timber packaging/dunnage used in the consignment has been (Please indicate below) Treated in compliance with Department of Agriculture and Water Resources treatment requirements" },
                    new DeclarationQuestionDto { QuestionNumber = 4, QuestionText = "statement to be removed from document when not relevant" },
                    new DeclarationQuestionDto { QuestionNumber = 5, QuestionText = "Issued Date" }
                };
            }
        }

        // GET Packaging List by Status
        public async Task<List<PackagingListDto>> GetPackagingListsByStatusAsync(string status)
        {
            try
            {
                _logger.LogInformation("Fetching packaging lists with status: {Status}", status);
                var packagingLists = await _context.PackagingLists
                    .Where(pl => pl.Status == status)
                    .OrderByDescending(pl => pl.CreatedAt)
                    .ToListAsync();

                return packagingLists.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching packaging lists with status: {Status}", status);
                throw;
            }
        }

        // GET Packaging List by Company
        public async Task<List<PackagingListDto>> GetPackagingListsByCompanyAsync(string company)
        {
            try
            {
                _logger.LogInformation("Fetching packaging lists for company: {Company}", company);
                var packagingLists = await _context.PackagingLists
                    .Where(pl => pl.Company == company)
                    .OrderByDescending(pl => pl.CreatedAt)
                    .ToListAsync();

                return packagingLists.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching packaging lists for company: {Company}", company);
                throw;
            }
        }

        // Helper method to map Model to DTO
        private PackagingListDto MapToDto(PackagingList packagingList)
        {
            return new PackagingListDto
            {
                Id = packagingList.Id,
                Company = packagingList.Company,
                Buyer = packagingList.Buyer,
                Style = packagingList.Style,
                PurchaseOrder = packagingList.PurchaseOrder,
                StyleBuyer = packagingList.StyleBuyer,
                ShippedBy = packagingList.ShippedBy,
                ShortDescription = packagingList.ShortDescription,
                Size = packagingList.Size,
                Color = packagingList.Color,
                Brand = packagingList.Brand,
                Packing = packagingList.Packing,
                PcsPerSet = packagingList.PcsPerSet,
                ProductNo = packagingList.ProductNo,
                CountryOfOrigin = packagingList.CountryOfOrigin,
                PortOfLoading = packagingList.PortOfLoading,
                CountryOfDestination = packagingList.CountryOfDestination,
                PortOfDischarge = packagingList.PortOfDischarge,
                Incoterm = packagingList.Incoterm,
                OrmsNo = packagingList.OrmsNo,
                LmkgNo = packagingList.LmkgNo,
                OrmsStyleNo = packagingList.OrmsStyleNo,
                Status = packagingList.Status,

                // Buyer Specific Fields
                ItemNo = packagingList.ItemNo,
                Pod = packagingList.Pod,
                Boi = packagingList.Boi,
                Wwwk = packagingList.Wwwk,
                NoOfColor = packagingList.NoOfColor,
                KeyCode = packagingList.KeyCode,
                SupplierCode = packagingList.SupplierCode,

                // Additional Fields
                Cartons = packagingList.Cartons,
                OrderQtyPac = packagingList.OrderQtyPac,
                OrderQtyPcs = packagingList.OrderQtyPcs,
                ShipQtyPac = packagingList.ShipQtyPac,
                ShipQtyPcs = packagingList.ShipQtyPcs,
                Destination = packagingList.Destination,

                PackingDetails = packagingList.GetPackingDetails().Select(pd => new PackingDetailDto
                {
                    Sl = pd.Sl,
                    NoOfCarton = pd.NoOfCarton,
                    Start = pd.Start,
                    End = pd.End,
                    SizeName = pd.SizeName,
                    Ratio = pd.Ratio,
                    ArticleNo = pd.ArticleNo,
                    PcsPack = pd.PcsPack,
                    PacCarton = pd.PacCarton,
                    OrderQty = pd.OrderQty,
                    TotalPcs = pd.TotalPcs,
                    TotalPacs = pd.TotalPacs,
                    GWt = pd.GWt,
                    NWt = pd.NWt,
                    TotalWt = pd.TotalWt,
                    L = pd.L,
                    W = pd.W,
                    H = pd.H,
                    CBM = pd.CBM
                }).ToList(),

                DeclarationAnswers = packagingList.GetDeclarationAnswers().Select(da => new DeclarationAnswerDto
                {
                    QuestionNumber = da.QuestionNumber,
                    Question = da.Question,
                    Answer = da.Answer,
                    AnswerDate = da.AnswerDate
                }).ToList(),

                CreatedAt = packagingList.CreatedAt,
                UpdatedAt = packagingList.UpdatedAt
            };
        }





public async Task<List<string>> GetPackagingListColumnsAsync()
{
    try
    {
        _logger.LogInformation("Getting column names for export model");

        // Get the property names of the PackingDetailForExport class
        var properties = typeof(PackingDetailForExport).GetProperties();

        // Return all property names
        var columnNames = properties
            .Select(p => p.Name)
            .ToList();

        return await Task.FromResult(columnNames);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error getting column names for export model");
        throw;
    }
}
        // Helper method to map PackingDetail DTOs to Model
        private List<PackingDetail> MapPackingDetails(List<PackingDetailDto> dtos)
        {
            return dtos.Select(dto => new PackingDetail
            {
                Sl = dto.Sl,
                NoOfCarton = dto.NoOfCarton,
                Start = dto.Start,
                End = dto.End,
                SizeName = dto.SizeName,
                Ratio = dto.Ratio,
                ArticleNo = dto.ArticleNo,
                PcsPack = dto.PcsPack,
                PacCarton = dto.PacCarton,
                OrderQty = dto.OrderQty,
                TotalPcs = dto.TotalPcs,
                TotalPacs = dto.TotalPacs,
                GWt = dto.GWt,
                NWt = dto.NWt,
                TotalWt = dto.TotalWt,
                L = dto.L,
                W = dto.W,
                H = dto.H,
                CBM = dto.CBM
            }).ToList();
        }

        // Helper method to map DeclarationAnswer DTOs to Model
        private List<DeclarationAnswer> MapDeclarationAnswers(List<DeclarationAnswerDto> dtos)
        {
            return dtos.Select(dto => new DeclarationAnswer
            {
                QuestionNumber = dto.QuestionNumber,
                Question = dto.Question,
                Answer = dto.Answer,
                AnswerDate = dto.AnswerDate
            }).ToList();
        }
    }
    


   

    // Additional DTO for declaration questions
    public class DeclarationQuestionDto
    {
        public int QuestionNumber { get; set; }
        public string QuestionText { get; set; } = string.Empty;
    }



    // Add this method to your PackagingListService.cs

       
}