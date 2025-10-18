// // Models/GatepassHeader.cs
// using System;
// using System.Collections.Generic;
// using System.ComponentModel.DataAnnotations;

// namespace DotNetWbapi.Models
// {
//     public class GatepassHeader
//     {
//         [Key]
//         public Guid Id { get; set; } = Guid.NewGuid();

//         [Required]
//         [StringLength(50)]
//         public string GatePassNo { get; set; } = string.Empty;

//         [Required]
//         public DateTime Date { get; set; } = DateTime.UtcNow;

//         [Required]
//         [StringLength(100)]
//         public string CompanyName { get; set; } = string.Empty;

//         [Required]
//         [StringLength(200)]
//         public string Address { get; set; } = string.Empty;

//         [Required]
//         [StringLength(100)]
//         public string IssuedTo { get; set; } = string.Empty;

//         [StringLength(500)]
//         public string Remarks { get; set; } = string.Empty;

//         public virtual ICollection<GatepassDetail> Details { get; set; } = new List<GatepassDetail>();
//     }
// }


// Models/GatepassHeader.cs
using System;
using System.Collections.Generic;

namespace DotNetWbapi.Models
{
    public class GatepassHeader
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


