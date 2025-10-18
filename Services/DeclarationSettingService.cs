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
    public class DeclarationSettingService
    {
        private readonly AppDBContext _context;
        private readonly ILogger<DeclarationSettingService> _logger;

        public DeclarationSettingService(AppDBContext context, ILogger<DeclarationSettingService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<DeclarationSettingDto?> GetDeclarationSettingByIdAsync(Guid id)
        {
            var setting = await _context.DeclarationSettings.FindAsync(id);
            if (setting == null) return null;

            return new DeclarationSettingDto
            {
                Id = setting.Id,
                CompanyName = setting.CompanyName,
                Questions = setting.GetQuestions()
            };
        }

        public async Task<List<DeclarationSettingDto>> GetAllDeclarationSettingsAsync()
        {
            return await _context.DeclarationSettings
                .Select(s => new DeclarationSettingDto
                {
                    Id = s.Id,
                    CompanyName = s.CompanyName,
                    Questions = s.GetQuestions()
                })
                .ToListAsync();
        }

       public async Task<DeclarationSettingDto> CreateDeclarationSettingAsync(CreateDeclarationSettingDto dto)
{
    _logger.LogInformation("Creating declaration setting for company: {Company}", dto.CompanyName);
    
    try
    {
        var setting = new DeclarationSetting
        {
            Id = Guid.NewGuid(),
            CompanyName = dto.CompanyName,
            CreatedAt = DateTime.UtcNow
        };
        setting.SetQuestions(dto.Questions);

        _context.DeclarationSettings.Add(setting);
        
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving declaration setting to database");
            throw new Exception("Failed to save declaration setting to database", ex);
        }

        return new DeclarationSettingDto
        {
            Id = setting.Id,
            CompanyName = setting.CompanyName,
            Questions = dto.Questions
        };
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error creating declaration setting");
        throw;
    }
}
        public async Task<DeclarationSettingDto?> UpdateDeclarationSettingAsync(Guid id, UpdateDeclarationSettingDto dto)
        {
            _logger.LogInformation("Updating declaration setting with ID: {Id}", id);
            
            var setting = await _context.DeclarationSettings.FindAsync(id);
            if (setting == null) return null;

            setting.CompanyName = dto.CompanyName;
            setting.SetQuestions(dto.Questions);
            setting.UpdatedAt = DateTime.UtcNow;

            _context.DeclarationSettings.Update(setting);
            await _context.SaveChangesAsync();

            return new DeclarationSettingDto
            {
                Id = setting.Id,
                CompanyName = setting.CompanyName,
                Questions = dto.Questions
            };
        }

        public async Task<bool> DeleteDeclarationSettingAsync(Guid id)
        {
            _logger.LogInformation("Deleting declaration setting with ID: {Id}", id);
            
            var setting = await _context.DeclarationSettings.FindAsync(id);
            if (setting == null) return false;

            _context.DeclarationSettings.Remove(setting);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}