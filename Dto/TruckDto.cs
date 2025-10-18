using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DotNetWbapi.Dtos
{
    public class TruckDto
    {
        public Guid Id { get; set; }
        
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
    }

    public class TruckResponseDto
    {
        public Guid Id { get; set; }
        public string TruckNumber { get; set; } = string.Empty;
        public string TruckLicense { get; set; } = string.Empty;
        public string TransportCompanyName { get; set; } = string.Empty;
        public string DriverName { get; set; } = string.Empty;
        public string DriverLicense { get; set; } = string.Empty;
        public string DriverPhone { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class TruckListDto
    {
        public List<TruckDto> Trucks { get; set; } = new List<TruckDto>();
    }
}