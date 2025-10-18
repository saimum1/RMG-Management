using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DotNetWbapi.Services;
using DotNetWbapi.Dtos;

namespace DotNetWbapi.Controllers
{
    [ApiController]
    [Route("api/packaging-list")]
    public class PackagingListController : ControllerBase
    {
        private readonly PackagingListService _packagingListService;
        private readonly ILogger<PackagingListController> _logger;

        public PackagingListController(
            PackagingListService packagingListService,
            ILogger<PackagingListController> logger)
        {
            _packagingListService = packagingListService;
            _logger = logger;
        }

        // GET: api/packaging-list
        [HttpGet]
        public async Task<IActionResult> GetPackagingLists()
        {
            try
            {
                _logger.LogInformation("Fetching all packaging lists");
                var packagingLists = await _packagingListService.GetAllPackagingListsAsync();
                return Ok(new { success = true, data = packagingLists });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all packaging lists");
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        // GET: api/packaging-list/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPackagingList(Guid id)
        {
            try
            {
                _logger.LogInformation("Fetching packaging list with ID: {Id}", id);
                var packagingList = await _packagingListService.GetPackagingListByIdAsync(id);
                
                if (packagingList == null)
                {
                    _logger.LogWarning("Packaging list with ID: {Id} not found", id);
                    return NotFound(new { success = false, message = "Packaging list not found" });
                }

                return Ok(new { success = true, data = packagingList });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching packaging list with ID: {Id}", id);
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        // GET: api/packaging-list/declaration-questions
        [HttpGet("declaration-questions")]
        public async Task<IActionResult> GetDeclarationQuestions()
        {
            try
            {
                _logger.LogInformation("Fetching declaration questions");
                var questions = await _packagingListService.GetDeclarationQuestionsAsync();
                return Ok(new { success = true, data = questions });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching declaration questions");
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        // GET: api/packaging-list/status/{status}
        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetPackagingListsByStatus(string status)
        {
            try
            {
                _logger.LogInformation("Fetching packaging lists with status: {Status}", status);
                var packagingLists = await _packagingListService.GetPackagingListsByStatusAsync(status);
                return Ok(new { success = true, data = packagingLists });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching packaging lists with status: {Status}", status);
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        // GET: api/packaging-list/company/{company}
        [HttpGet("company/{company}")]
        public async Task<IActionResult> GetPackagingListsByCompany(string company)
        {
            try
            {
                _logger.LogInformation("Fetching packaging lists for company: {Company}", company);
                var packagingLists = await _packagingListService.GetPackagingListsByCompanyAsync(company);
                return Ok(new { success = true, data = packagingLists });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching packaging lists for company: {Company}", company);
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        // POST: api/packaging-list
        [HttpPost]
        public async Task<IActionResult> CreatePackagingList([FromBody] CreatePackagingListDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for create packaging list");
                    return BadRequest(new { success = false, message = "Invalid data", errors = ModelState.Values });
                }

                _logger.LogInformation("Creating packaging list for company: {Company}, buyer: {Buyer}", dto.Company, dto.Buyer);
                var newPackagingList = await _packagingListService.CreatePackagingListAsync(dto);
                
                return CreatedAtAction(
                    nameof(GetPackagingList), 
                    new { id = newPackagingList.Id }, 
                    new { success = true, data = newPackagingList });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating packaging list for company: {Company}, buyer: {Buyer}", dto.Company, dto.Buyer);
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        // PUT: api/packaging-list/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePackagingList(Guid id, [FromBody] UpdatePackagingListDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for update packaging list with ID: {Id}", id);
                    return BadRequest(new { success = false, message = "Invalid data", errors = ModelState.Values });
                }

                _logger.LogInformation("Updating packaging list with ID: {Id}", id);
                var updatedPackagingList = await _packagingListService.UpdatePackagingListAsync(id, dto);
                
                if (updatedPackagingList == null)
                {
                    _logger.LogWarning("Packaging list with ID: {Id} not found for update", id);
                    return NotFound(new { success = false, message = "Packaging list not found" });
                }

                return Ok(new { success = true, data = updatedPackagingList });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating packaging list with ID: {Id}", id);
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        // DELETE: api/packaging-list/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePackagingList(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting packaging list with ID: {Id}", id);
                var result = await _packagingListService.DeletePackagingListAsync(id);

                if (!result)
                {
                    _logger.LogWarning("Packaging list with ID: {Id} not found for deletion", id);
                    return NotFound(new { success = false, message = "Packaging list not found" });
                }

                return Ok(new { success = true, message = "Packaging list deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting packaging list with ID: {Id}", id);
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }



        [HttpGet("columns")]
        public async Task<IActionResult> GetPackagingListColumns()
        {
            try
            {
                _logger.LogInformation("Fetching column names for PackagingList table");
                var columnNames = await _packagingListService.GetPackagingListColumnsAsync();
                return Ok(new { success = true, data = columnNames });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching column names for PackagingList table");
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        // PATCH: api/packaging-list/{id}/status
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateStatusDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Status))
                {
                    return BadRequest(new { success = false, message = "Status is required" });
                }

                _logger.LogInformation("Updating status for packaging list with ID: {Id} to {Status}", id, dto.Status);
                
                // Get the existing packaging list
                var existingList = await _packagingListService.GetPackagingListByIdAsync(id);
                if (existingList == null)
                {
                    _logger.LogWarning("Packaging list with ID: {Id} not found for status update", id);
                    return NotFound(new { success = false, message = "Packaging list not found" });
                }

                // Create update DTO with only the status changed
                var updateDto = new UpdatePackagingListDto
                {
                    Company = existingList.Company,
                    Buyer = existingList.Buyer,
                    Style = existingList.Style,
                    PurchaseOrder = existingList.PurchaseOrder,
                    StyleBuyer = existingList.StyleBuyer,
                    ShippedBy = existingList.ShippedBy,
                    ShortDescription = existingList.ShortDescription,
                    Size = existingList.Size,
                    Color = existingList.Color,
                    Brand = existingList.Brand,
                    Packing = existingList.Packing,
                    PcsPerSet = existingList.PcsPerSet,
                    ProductNo = existingList.ProductNo,
                    CountryOfOrigin = existingList.CountryOfOrigin,
                    PortOfLoading = existingList.PortOfLoading,
                    CountryOfDestination = existingList.CountryOfDestination,
                    PortOfDischarge = existingList.PortOfDischarge,
                    Incoterm = existingList.Incoterm,
                    OrmsNo = existingList.OrmsNo,
                    LmkgNo = existingList.LmkgNo,
                    OrmsStyleNo = existingList.OrmsStyleNo,
                    Status = dto.Status,
                    
                    // Buyer Specific Fields
                    ItemNo = existingList.ItemNo,
                    Pod = existingList.Pod,
                    Boi = existingList.Boi,
                    Wwwk = existingList.Wwwk,
                    NoOfColor = existingList.NoOfColor,
                    KeyCode = existingList.KeyCode,
                    SupplierCode = existingList.SupplierCode,
                    
                    // Additional Fields
                    Cartons = existingList.Cartons,
                    OrderQtyPac = existingList.OrderQtyPac,
                    OrderQtyPcs = existingList.OrderQtyPcs,
                    ShipQtyPac = existingList.ShipQtyPac,
                    ShipQtyPcs = existingList.ShipQtyPcs,
                    Destination = existingList.Destination,
                    
                    PackingDetails = existingList.PackingDetails,
                    DeclarationAnswers = existingList.DeclarationAnswers
                };

                var updatedPackagingList = await _packagingListService.UpdatePackagingListAsync(id, updateDto);
                
                return Ok(new { success = true, data = updatedPackagingList });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating status for packaging list with ID: {Id}", id);
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }
    }

    // DTO for status update
    public class UpdateStatusDto
    {
        public string Status { get; set; } = string.Empty;
    }



    // Add this endpoint to your PackagingListController.cs

    
}