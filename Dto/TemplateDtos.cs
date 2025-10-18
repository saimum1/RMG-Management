// TemplateDtos.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DotNetWbapi.Dtos
{
    public class TemplateDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<TemplateMappingDto> Mappings { get; set; } = new();
    }

    public class TemplateMappingDto
    {
        public Guid Id { get; set; }
        public string ExcelColumn { get; set; } = string.Empty;
        public string DatabaseField { get; set; } = string.Empty;
        public bool IsRecurring { get; set; }
    }

    public class CreateTemplateDto
    {
        [Required(ErrorMessage = "Template name is required")]
        [StringLength(100, ErrorMessage = "Template name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "File is required")]
        public IFormFile File { get; set; } = null!;

        public bool IsActive { get; set; } = false;

        [Required(ErrorMessage = "At least one mapping is required")]
        [MinLength(1, ErrorMessage = "At least one mapping is required")]
        public List<CreateTemplateMappingDto> Mappings { get; set; } = new();
    }

    public class CreateTemplateMappingDto
    {
        [Required(ErrorMessage = "Excel column is required")]
        [StringLength(100, ErrorMessage = "Excel column cannot exceed 100 characters")]
        public string ExcelColumn { get; set; } = string.Empty;

        [Required(ErrorMessage = "Database field is required")]
        [StringLength(100, ErrorMessage = "Database field cannot exceed 100 characters")]
        public string DatabaseField { get; set; } = string.Empty;

        public bool IsRecurring { get; set; } = false;
    }
    

    public class DatabaseColumnDto
    {
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
    }
}