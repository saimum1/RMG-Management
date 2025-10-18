// TemplateController.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DotNetWbapi.Services;
using DotNetWbapi.Dtos;

namespace DotNetWbapi.Controllers
{

    // declare the API controller with route prefix "api/template"
    [ApiController]
    [Route("api/template")]
    public class TemplateController : ControllerBase
    {
        private readonly TemplateService _templateService;
        private readonly ILogger<TemplateController> _logger;

        public TemplateController(
            TemplateService templateService,
            ILogger<TemplateController> logger)
        {
            _templateService = templateService;
            _logger = logger;
        }

        // GET: api/template
        // Fetches all templates
        [HttpGet]
        public async Task<IActionResult> GetTemplates()
        {
            try
            {
                _logger.LogInformation("Fetching all templates");
                var templates = await _templateService.GetAllTemplatesAsync();
                return Ok(new { success = true, data = templates });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all templates");
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        // GET: api/template/{id}
        // Fetches a specific template by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTemplate(Guid id)
        {
            try
            {
                _logger.LogInformation("Fetching template with ID: {Id}", id);
                var template = await _templateService.GetTemplateByIdAsync(id);

                if (template == null)
                {
                    _logger.LogWarning("Template with ID: {Id} not found", id);
                    return NotFound(new { success = false, message = "Template not found" });
                }

                return Ok(new { success = true, data = template });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching template with ID: {Id}", id);
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        // POST: api/template
        // Creates a new template
        [HttpPost]
        public async Task<IActionResult> CreateTemplate([FromForm] CreateTemplateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for create template");
                    return BadRequest(new { success = false, message = "Invalid data", errors = ModelState.Values });
                }

                _logger.LogInformation("Creating template: {Name}", dto.Name);
                var newTemplate = await _templateService.CreateTemplateAsync(dto);

                return CreatedAtAction(
                    nameof(GetTemplate),
                    new { id = newTemplate.Id },
                    new { success = true, data = newTemplate });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating template: {Name}", dto.Name);
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        // GET: api/template/{id}/download
        // Fetches a specific template by ID
        [HttpGet("{id}/download")]
        public async Task<IActionResult> DownloadTemplateWithDataa(Guid id)
        {
            try
            {
                _logger.LogInformation("Downloading template with data for ID: {Id}", id);
                var excelBytes = await _templateService.GenerateExcelWithDataAsync(id);

                var template = await _templateService.GetTemplateByIdAsync(id);
                if (template == null)
                {
                    _logger.LogWarning("Template with ID: {Id} not found", id);
                    return NotFound(new { success = false, message = "Template not found" });
                }

                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                           $"{template.Name}_WithData.xlsx");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading template with data for ID: {Id}", id);
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }


        // GET: api/template/{id}/download-with-data
        // GET: api/template/{id}/download-with-data
        [HttpGet("{id}/download-with-data")]
        public async Task<IActionResult> DownloadTemplateWithData(Guid id)
        {
            try
            {
                _logger.LogInformation("Downloading template with data for ID: {Id}", id);

                // Get the packing list ID from the query parameters
                var packingListIdStr = Request.Query["packingListId"];
                if (!Guid.TryParse(packingListIdStr, out var packingListId))
                {
                    return BadRequest(new { success = false, message = "Valid packing list ID is required" });
                }

                var excelBytes = await _templateService.GenerateExcelWithDataAsync(id, packingListId);

                var template = await _templateService.GetTemplateByIdAsync(id);
                if (template == null)
                {
                    _logger.LogWarning("Template with ID: {Id} not found", id);
                    return NotFound(new { success = false, message = "Template not found" });
                }

                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                           $"{template.Name}_WithData.xlsx");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading template with data for ID: {Id}", id);
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }



        // GET: api/template/packaging-list/columns
        // Fetches column names for PackagingList table
        [HttpGet("packaging-list/columns")]
        public async Task<IActionResult> GetPackagingListColumns()
        {
            try
            {
                _logger.LogInformation("Fetching packaging list columns");
                var columns = await _templateService.GetDatabaseColumnsAsync();
                return Ok(new { success = true, data = columns });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching packaging list columns");
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }

        // PUT: api/template/{id}
        // Updates a specific template by ID
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTemplate(Guid id, [FromForm] CreateTemplateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for update template with ID: {Id}", id);
                    return BadRequest(new { success = false, message = "Invalid data", errors = ModelState.Values });
                }

                _logger.LogInformation("Updating template with ID: {Id}", id);
                var updatedTemplate = await _templateService.UpdateTemplateAsync(id, dto);

                if (updatedTemplate == null)
                {
                    _logger.LogWarning("Template with ID: {Id} not found for update", id);
                    return NotFound(new { success = false, message = "Template not found" });
                }

                return Ok(new { success = true, data = updatedTemplate });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating template with ID: {Id}", id);
                return StatusCode(500, new { success = false, message = "Internal server error", error = ex.Message });
            }
        }
        

        // DELETE: api/template/{id}
        // Deletes a specific template by ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTemplate(Guid id)
        {
            try
            {
                var result = await _templateService.DeleteTemplateAsync(id);
                
                if (result)
                {
                    return Ok(new { success = true, message = "Template deleted successfully" });
                }
                else
                {
                    return NotFound(new { success = false, message = "Template not found" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting template with ID: {Id}", id);
                return StatusCode(500, new { success = false, message = "Error deleting template" });
            }
        }
    }
}