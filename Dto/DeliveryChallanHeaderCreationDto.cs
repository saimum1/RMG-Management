using System;
using System.Collections.Generic;

namespace DotNetWbapi.Dtos
{
    public class DeliveryChallanHeaderCreationDto
    {
        public Guid Id { get; set; }
        public string ChallanNo { get; set; } = string.Empty;
        public string ExistingTruckNo { get; set; } = string.Empty;
        public string? HireTruckNo { get; set; } = string.Empty;
        public DateTime InDate { get; set; }
        public string InTime { get; set; } = string.Empty;
        public string To { get; set; } = string.Empty;
        public string DriverName { get; set; } = string.Empty;
        // public DateTime? OutDate { get; set; } = null; 
        public string? OutDate { get; set; } = string.Empty;
        
        public string? OutTime { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string LicenseNo { get; set; } = string.Empty;
        public string TruckCBM { get; set; } = string.Empty;
        public string DepotName { get; set; } = string.Empty;
        public string MobileNo { get; set; } = string.Empty;
        public string TransportCompany { get; set; } = string.Empty;
        public string LockNo { get; set; } = string.Empty;
        public string? RentedAmount { get; set; } = string.Empty;
        public string PoNo { get; set; } = string.Empty;

        public string? agdl { get; set; } = null;
        public string? descriptions { get; set; } = null;
        public string? remarks { get; set; } = null;
        public string? quantity { get; set; } = null;
        public string? unit { get; set; } = null;

    }
}