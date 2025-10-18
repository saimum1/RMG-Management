using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DotNetWbapi.Models
{
    public class Truck
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(50)]
        public string TruckNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string TruckLicense { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string TransportCompanyName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string DriverName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string DriverLicense { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string DriverPhone { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}