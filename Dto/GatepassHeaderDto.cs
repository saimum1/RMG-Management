// // Dtos/GatepassHeaderDto.cs
// using System;
// using System.Collections.Generic;

// namespace DotNetWbapi.Dtos
// {
//     public class GatepassHeaderDto
//     {
//         public Guid Id { get; set; }
//         public string GatePassNo { get; set; } = string.Empty;
//         public DateTime Date { get; set; }
//         public string CompanyName { get; set; } = string.Empty;
//         public string Address { get; set; } = string.Empty;
//         public string IssuedTo { get; set; } = string.Empty;
//         public string Remarks { get; set; } = string.Empty;
//         public List<GatepassDetailDto> Details { get; set; } = new List<GatepassDetailDto>();
//     }
// }


// Dtos/GatepassHeaderDto.cs
using System;
using System.Collections.Generic;

namespace DotNetWbapi.Dtos
{
    public class GatepassHeaderDto
    {
        public Guid Id { get; set; }
        public string GatePassNo { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string IssuedTo { get; set; } = string.Empty;
        public string TruckNo { get; set; } = string.Empty;
        public string DriverName { get; set; } = string.Empty;
        public string TransportCompany { get; set; } = string.Empty;
        public string DriverPhone { get; set; } = string.Empty;
        public string DriverLicense { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
    }
}