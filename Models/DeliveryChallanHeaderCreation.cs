using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DotNetWbapi.Models
{
    public class DeliveryChallanHeaderCreation
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(50)]
        public string ChallanNo { get; set; } = string.Empty;

        [StringLength(50)]
        public string? ExistingTruckNo { get; set; } = string.Empty;

        [StringLength(50)]
        public string? HireTruckNo { get; set; } = null;

        [Required]
        public DateTime InDate { get; set; }

        [Required]
        [StringLength(8)]
        public string InTime { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string To { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string DriverName { get; set; } = string.Empty;

        public string? OutDate { get; set; } = string.Empty;
        // public DateTime? OutDate { get; set; } = null; 

        [StringLength(8)]
        public string? OutTime { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LicenseNo { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string TruckCBM { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string DepotName { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string MobileNo { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string TransportCompany { get; set; } = string.Empty;

        [StringLength(50)]
        public string LockNo { get; set; } = string.Empty;

        [StringLength(50)]
        public string? RentedAmount { get; set; } = null;
        public string? agdl { get; set; } = null;
        public string? descriptions { get; set; } = null;
        public string? remarks { get; set; } = null;
        public string? quantity { get; set; } = null;
        public string? unit { get; set; } = null;

    //     public virtual ICollection<DeliveryChallanDetailCreation> CBMDetails { get; set; } = new List<DeliveryChallanDetailCreation>();
    }
}