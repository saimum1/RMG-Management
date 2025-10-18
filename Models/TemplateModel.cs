// TemplateModel.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotNetWbapi.Models
{
    public class Template
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public byte[] FileContent { get; set; } = Array.Empty<byte>();

        [Required]
        [StringLength(255)]
        public string FileName { get; set; } = string.Empty;

        public bool IsActive { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation property for mappings
        public virtual ICollection<TemplateMapping> Mappings { get; set; } = new List<TemplateMapping>();
    }

    public class TemplateMapping
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid TemplateId { get; set; }

        [Required]
        [StringLength(100)]
        public string ExcelColumn { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string DatabaseField { get; set; } = string.Empty;

        public bool IsRecurring { get; set; } = false;

        // Navigation property
        [ForeignKey("TemplateId")]
        public virtual Template Template { get; set; } = null!;
    }
}