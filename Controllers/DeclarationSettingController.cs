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
    [Route("api/declaration-settings")]
    public class DeclarationSettingController : ControllerBase
    {
        private readonly DeclarationSettingService _declarationSettingService;
        private readonly ILogger<DeclarationSettingController> _logger;

        public DeclarationSettingController(
            DeclarationSettingService declarationSettingService,
            ILogger<DeclarationSettingController> logger)
        {
            _declarationSettingService = declarationSettingService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetDeclarationSettings()
        {
            _logger.LogInformation("Fetching all declaration settings");
            var settings = await _declarationSettingService.GetAllDeclarationSettingsAsync();
            return Ok(settings);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDeclarationSetting(Guid id)
        {
            _logger.LogInformation("Fetching declaration setting with ID: {Id}", id);
            var setting = await _declarationSettingService.GetDeclarationSettingByIdAsync(id);
            
            if (setting == null)
            {
                return NotFound();
            }

            return Ok(setting);
        }

        [HttpPost]
        public async Task<IActionResult> CreateDeclarationSetting([FromBody] CreateDeclarationSettingDto dto)
        {
            try
            {
                _logger.LogInformation("Creating declaration setting for company: {Company}", dto.CompanyName);
                var newSetting = await _declarationSettingService.CreateDeclarationSettingAsync(dto);
                return CreatedAtAction(nameof(GetDeclarationSetting), new { id = newSetting.Id }, newSetting);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating declaration setting");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDeclarationSetting(Guid id, [FromBody] UpdateDeclarationSettingDto dto)
        {
            try
            {
                _logger.LogInformation("Updating declaration setting with ID: {Id}", id);
                var updatedSetting = await _declarationSettingService.UpdateDeclarationSettingAsync(id, dto);
                
                if (updatedSetting == null)
                {
                    return NotFound();
                }

                return Ok(updatedSetting);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating declaration setting");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDeclarationSetting(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting declaration setting with ID: {Id}", id);
                var result = await _declarationSettingService.DeleteDeclarationSettingAsync(id);

                if (!result)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting declaration setting");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}