// TemplateService.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DotNetWbapi.Models;
using DotNetWbapi.Dtos;
using DotNetWbapi.Data;
using ClosedXML.Excel;
using System.Text.RegularExpressions;
using OfficeOpenXml;
using System.Reflection;

namespace DotNetWbapi.Services
{

    // Services/TemplateService.cs
    //feature 2.3.2 Packing List Excel Template Management reference from asignment doc file
    public class TemplateService
    {
        private readonly AppDBContext _context;
        private readonly ILogger<TemplateService> _logger;
        private readonly PackagingListService _packagingListService;

        public TemplateService(
            AppDBContext context,
            ILogger<TemplateService> logger,
            PackagingListService packagingListService)
        {
            _context = context;
            _logger = logger;
            _packagingListService = packagingListService;
        }




        // GET: api/template
        // Fetches all templates
        public async Task<List<TemplateDto>> GetAllTemplatesAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all templates");
                var templates = await _context.Templates
                    .Include(t => t.Mappings)
                    .OrderByDescending(t => t.CreatedAt)
                    .ToListAsync();

                return templates.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all templates");
                throw;
            }
        }


        // GET: api/template/{id}
        // Fetches a specific template by ID
        public async Task<TemplateDto?> GetTemplateByIdAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Fetching template with ID: {Id}", id);
                var template = await _context.Templates
                    .Include(t => t.Mappings)
                    .FirstOrDefaultAsync(t => t.Id == id);

                if (template == null)
                {
                    _logger.LogWarning("Template with ID: {Id} not found", id);
                    return null;
                }

                return MapToDto(template);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching template with ID: {Id}", id);
                throw;
            }
        }


        // POST: api/template
        // Creates a new template
        public async Task<TemplateDto> CreateTemplateAsync(CreateTemplateDto dto)
        {
            try
            {
                _logger.LogInformation("Creating new template: {Name}", dto.Name);

                using var memoryStream = new MemoryStream();
                await dto.File.CopyToAsync(memoryStream);
                var fileContent = memoryStream.ToArray();

                var template = new Template
                {
                    Name = dto.Name,
                    FileContent = fileContent,
                    FileName = dto.File.FileName,
                    IsActive = dto.IsActive
                };

                // Add mappings
                foreach (var mappingDto in dto.Mappings)
                {
                    template.Mappings.Add(new TemplateMapping
                    {
                        ExcelColumn = mappingDto.ExcelColumn,
                        DatabaseField = mappingDto.DatabaseField,
                        IsRecurring = mappingDto.IsRecurring
                    });
                }

                _context.Templates.Add(template);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Template created successfully with ID: {Id}", template.Id);
                return MapToDto(template);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating template: {Name}", dto.Name);
                throw;
            }
        }



        // PUT: api/template/{id}
        // Updates an existing template
        public async Task<byte[]> GenerateExcelWithDataAsync(Guid templateId)
        {
            try
            {
                _logger.LogInformation("Generating Excel with data for template ID: {Id}", templateId);

                var template = await _context.Templates
                    .Include(t => t.Mappings)
                    .FirstOrDefaultAsync(t => t.Id == templateId);

                if (template == null)
                {
                    _logger.LogWarning("Template with ID: {Id} not found", templateId);
                    throw new Exception("Template not found");
                }

                // Get all packaging lists
                var packagingLists = await _packagingListService.GetAllPackagingListsAsync();

                // Load the template
                using var outputStream = new MemoryStream();
                using var templateStream = new MemoryStream(template.FileContent);

                // Use EPPlus for Excel operations
                using var package = new ExcelPackage(templateStream);
                var worksheet = package.Workbook.Worksheets[0];

                // Find the header row (first row with data)
                int headerRow = 1;

                // Create a dictionary to map Excel column names to column indices
                var excelColumnMap = new Dictionary<string, int>();

                // First, let's identify all the column headers in the template
                for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
                {
                    var headerValue = worksheet.Cells[headerRow, col].Text?.Trim();
                    if (!string.IsNullOrEmpty(headerValue))
                    {
                        excelColumnMap[headerValue] = col;
                    }
                }

                // Now, let's populate the data starting from the row after the header
                int dataRow = headerRow + 1;

                foreach (var packagingList in packagingLists)
                {
                    // For each mapping, find the corresponding Excel column and populate the data
                    foreach (var mapping in template.Mappings)
                    {
                        if (excelColumnMap.TryGetValue(mapping.ExcelColumn, out int colIndex))
                        {
                            var value = GetPropertyValue(packagingList, mapping.DatabaseField);

                            // Handle different types of values
                            if (value == null)
                            {
                                worksheet.Cells[dataRow, colIndex].Value = "";
                            }
                            else if (value is DateTime dateValue)
                            {
                                worksheet.Cells[dataRow, colIndex].Value = dateValue;
                                worksheet.Cells[dataRow, colIndex].Style.Numberformat.Format = "yyyy-mm-dd";
                            }
                            else if (value is bool boolValue)
                            {
                                worksheet.Cells[dataRow, colIndex].Value = boolValue ? "Yes" : "No";
                            }
                            else
                            {
                                worksheet.Cells[dataRow, colIndex].Value = value.ToString();
                            }
                        }
                    }

                    dataRow++;
                }

                // Save the modified workbook
                package.SaveAs(outputStream);
                return outputStream.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating Excel with data for template ID: {Id}", templateId);
                throw;
            }
        }



        // PUT: api/template/{id}
        // Updates an existing template
        public async Task<bool> UpdateTemplateAsync(Guid id, CreateTemplateDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Fetch the template with its mappings
                var template = await _context.Templates
                    .Include(t => t.Mappings)  // Changed from TemplateMappings to Mappings
                    .FirstOrDefaultAsync(t => t.Id == id);

                if (template == null)
                {
                    return false;
                }

                // Update template properties
                template.Name = dto.Name;
                // Note: We're not updating FileContent and FileName here as they're not in the DTO
                template.IsActive = dto.IsActive;
                template.UpdatedAt = DateTime.UtcNow;

                // Handle template mappings
                // First, remove all existing mappings
                var existingMappings = template.Mappings.ToList();  // Changed from TemplateMappings to Mappings
                _context.TemplateMappings.RemoveRange(existingMappings);

                // Add new mappings from the DTO
                foreach (var mappingDto in dto.Mappings)
                {
                    var newMapping = new TemplateMapping
                    {
                        Id = Guid.NewGuid(), // Generate new ID
                        DatabaseField = mappingDto.DatabaseField,
                        ExcelColumn = mappingDto.ExcelColumn,
                        IsRecurring = mappingDto.IsRecurring,
                        TemplateId = template.Id
                    };
                    _context.TemplateMappings.Add(newMapping);
                }

                // Save changes
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error updating template with ID: {TemplateId}", id);
                throw;
            }
        }



        // DELETE: api/template/{id}
        // Deletes a template
        public async Task<bool> DeleteTemplateAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting template with ID: {Id}", id);

                var template = await _context.Templates
                    .Include(t => t.Mappings)
                    .FirstOrDefaultAsync(t => t.Id == id);

                if (template == null)
                {
                    _logger.LogWarning("Template with ID: {Id} not found", id);
                    return false;
                }

                // Remove the template (this will also remove related mappings due to cascade delete)
                _context.Templates.Remove(template);

                await _context.SaveChangesAsync();

                _logger.LogInformation("Template deleted successfully with ID: {Id}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting template with ID: {Id}", id);
                throw;
            }
        }



        // GET: api/template/{id}/download-with-data
        // GET: api/template/{id}/download-with-data
        public async Task<byte[]> GenerateExcelWithDataAsync(Guid templateId, Guid packingListId)
        {
            try
            {
                _logger.LogInformation("Generating Excel with data for template ID: {TemplateId} and packing list ID: {PackingListId}", templateId, packingListId);

                // Get template with mappings
                var template = await _context.Templates
                    .Include(t => t.Mappings)
                    .FirstOrDefaultAsync(t => t.Id == templateId);

                if (template == null)
                {
                    throw new Exception($"Template with ID {templateId} not found");
                }

                // Get packaging list
                var packagingList = await _context.PackagingLists
                    .FirstOrDefaultAsync(pl => pl.Id == packingListId);

                if (packagingList == null)
                {
                    throw new Exception($"Packaging list with ID {packingListId} not found");
                }

                // Get packing details from JSON
                var packingDetails = packagingList.GetPackingDetails();

                // Define the packing detail field names (PascalCase)
                var packingDetailFields = new List<string>
        {
            "Sl", "NoOfCarton", "Start", "End", "SizeName", "Ratio", "ArticleNo",
            "PcsPack", "PacCarton", "OrderQty", "TotalPcs", "TotalPacs", "GWt",
            "NWt", "TotalWt", "L", "W", "H", "CBM"
        };

                // Separate mappings into main and detail
                var mainMappings = template.Mappings
                    .Where(m => !packingDetailFields.Contains(m.DatabaseField))
                    .ToList();

                var detailMappings = template.Mappings
                    .Where(m => packingDetailFields.Contains(m.DatabaseField))
                    .ToList();

                // Create a new Excel package
                using (var package = new ExcelPackage())
                {
                    // Create main data worksheet
                    var mainWs = package.Workbook.Worksheets.Add("Main Data");

                    // Add header row for main data
                    for (int i = 0; i < mainMappings.Count; i++)
                    {
                        mainWs.Cells[1, i + 1].Value = mainMappings[i].ExcelColumn;
                    }
                    // Add Packing List ID column
                    mainWs.Cells[1, mainMappings.Count + 1].Value = "Packing List ID";

                    // Add data row for main data
                    for (int i = 0; i < mainMappings.Count; i++)
                    {
                        var mapping = mainMappings[i];
                        // Convert PascalCase to camelCase
                        var camelCaseProperty = char.ToLower(mapping.DatabaseField[0]) + mapping.DatabaseField.Substring(1);
                        var propertyValue = packagingList.GetType().GetProperty(camelCaseProperty)?.GetValue(packagingList);
                        mainWs.Cells[2, i + 1].Value = propertyValue?.ToString() ?? "";
                    }
                    // Add packing list ID - Now using the parameter
                    mainWs.Cells[2, mainMappings.Count + 1].Value = packingListId.ToString();

                    // Create packing details worksheet if there are detail mappings
                    if (detailMappings.Any())
                    {
                        var detailWs = package.Workbook.Worksheets.Add("Packing Details");

                        // Add header row for packing details
                        for (int i = 0; i < detailMappings.Count; i++)
                        {
                            detailWs.Cells[1, i + 1].Value = detailMappings[i].ExcelColumn;
                        }

                        // Add data rows for packing details
                        for (int row = 0; row < packingDetails.Count; row++)
                        {
                            var detail = packingDetails[row];
                            for (int col = 0; col < detailMappings.Count; col++)
                            {
                                var mapping = detailMappings[col];
                                // Convert PascalCase to camelCase
                                var camelCaseProperty = char.ToLower(mapping.DatabaseField[0]) + mapping.DatabaseField.Substring(1);
                                var propertyValue = detail.GetType().GetProperty(camelCaseProperty)?.GetValue(detail);
                                detailWs.Cells[row + 2, col + 1].Value = propertyValue?.ToString() ?? "";
                            }
                        }
                    }

                    // Return the Excel file as byte array
                    return package.GetAsByteArray();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating Excel with data for template ID: {TemplateId} and packing list ID: {PackingListId}", templateId, packingListId);
                throw;
            }
        }


        // GET: api/template/packaging-list/columns
        // Fetches column names for PackagingList table
        public async Task<List<DatabaseColumnDto>> GetDatabaseColumnsAsync()
        {
            try
            {
                _logger.LogInformation("Fetching database columns");

                // Get all properties of PackagingListDto
                var properties = typeof(PackagingListDto).GetProperties();

                var columns = properties.Select(p =>
                {
                    string displayName = System.Text.RegularExpressions.Regex.Replace(p.Name, "([A-Z])", " $1").TrimStart();
                    if (!string.IsNullOrEmpty(displayName))
                    {
                        displayName = char.ToUpper(displayName[0]) + displayName.Substring(1);
                    }

                    return new DatabaseColumnDto
                    {
                        Name = p.Name,
                        DisplayName = displayName
                    };
                }).ToList();

                return columns;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching database columns");
                throw;
            }
        }
            

        // Helper method to map Template to TemplateDto
            
        private TemplateDto MapToDto(Template template)
        {
            return new TemplateDto
            {
                Id = template.Id,
                Name = template.Name,
                FileName = template.FileName,
                IsActive = template.IsActive,
                CreatedAt = template.CreatedAt,
                UpdatedAt = template.UpdatedAt,
                Mappings = template.Mappings.Select(m => new TemplateMappingDto
                {
                    Id = m.Id,
                    ExcelColumn = m.ExcelColumn,
                    DatabaseField = m.DatabaseField,
                    IsRecurring = m.IsRecurring
                }).ToList()
            };
        }

        private object? GetPropertyValue(PackagingListDto obj, string propertyName)
            {
                return obj.GetType().GetProperty(propertyName)?.GetValue(obj, null);
            }
    }
}