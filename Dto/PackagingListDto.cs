using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DotNetWbapi.Dtos
{
    public class PackagingListDto
    {
        public Guid Id { get; set; }
        public string Company { get; set; } = string.Empty;
        public string Buyer { get; set; } = string.Empty;
        public string Style { get; set; } = string.Empty;
        public string PurchaseOrder { get; set; } = string.Empty;
        public string StyleBuyer { get; set; } = string.Empty;
        public string ShippedBy { get; set; } = string.Empty;
        public string ShortDescription { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Packing { get; set; } = string.Empty;
        public string PcsPerSet { get; set; } = string.Empty;
        public string ProductNo { get; set; } = string.Empty;
        public string CountryOfOrigin { get; set; } = string.Empty;
        public string PortOfLoading { get; set; } = string.Empty;
        public string CountryOfDestination { get; set; } = string.Empty;
        public string PortOfDischarge { get; set; } = string.Empty;
        public string Incoterm { get; set; } = string.Empty;
        public string OrmsNo { get; set; } = string.Empty;
        public string LmkgNo { get; set; } = string.Empty;
        public string OrmsStyleNo { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        
        // Buyer Specific Fields
        public string ItemNo { get; set; } = string.Empty;
        public string Pod { get; set; } = string.Empty;
        public string Boi { get; set; } = string.Empty;
        public string Wwwk { get; set; } = string.Empty;
        public string NoOfColor { get; set; } = string.Empty;
        public string KeyCode { get; set; } = string.Empty;
        public string SupplierCode { get; set; } = string.Empty;
        
        // Additional Fields
        public string Cartons { get; set; } = string.Empty;
        public string OrderQtyPac { get; set; } = string.Empty;
        public string OrderQtyPcs { get; set; } = string.Empty;
        public string ShipQtyPac { get; set; } = string.Empty;
        public string ShipQtyPcs { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        
        public List<PackingDetailDto> PackingDetails { get; set; } = new();
        public List<DeclarationAnswerDto> DeclarationAnswers { get; set; } = new();
        
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class PackingDetailDto
    {
        public int Sl { get; set; }
        public string NoOfCarton { get; set; } = string.Empty;
        public string Start { get; set; } = string.Empty;
        public string End { get; set; } = string.Empty;
        public string SizeName { get; set; } = string.Empty;
        public string Ratio { get; set; } = string.Empty;
        public string ArticleNo { get; set; } = string.Empty;
        public string PcsPack { get; set; } = string.Empty;
        public string PacCarton { get; set; } = string.Empty;
        public string OrderQty { get; set; } = string.Empty;
        public string TotalPcs { get; set; } = string.Empty;
        public string TotalPacs { get; set; } = string.Empty;
        public string GWt { get; set; } = string.Empty;
        public string NWt { get; set; } = string.Empty;
        public string TotalWt { get; set; } = string.Empty;
        public string L { get; set; } = string.Empty;
        public string W { get; set; } = string.Empty;
        public string H { get; set; } = string.Empty;
        public string CBM { get; set; } = string.Empty;
    }

    public class DeclarationAnswerDto
    {
        public int QuestionNumber { get; set; }
        public string Question { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public DateTime? AnswerDate { get; set; }
    }

    public class CreatePackagingListDto
    {
        [Required(ErrorMessage = "Company is required")]
        [StringLength(100)]
        public string Company { get; set; } = string.Empty;

        [Required(ErrorMessage = "Buyer is required")]
        [StringLength(100)]
        public string Buyer { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string Style { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string PurchaseOrder { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string StyleBuyer { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string ShippedBy { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string ShortDescription { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string Size { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string Color { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string Brand { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string Packing { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string PcsPerSet { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string ProductNo { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string CountryOfOrigin { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string PortOfLoading { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string CountryOfDestination { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string PortOfDischarge { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string Incoterm { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string OrmsNo { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string LmkgNo { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string OrmsStyleNo { get; set; } = string.Empty;
        
        [StringLength(20)]
        public string Status { get; set; } = "draft";
        
        // Buyer Specific Fields
        [StringLength(50)]
        public string ItemNo { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string Pod { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string Boi { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string Wwwk { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string NoOfColor { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string KeyCode { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string SupplierCode { get; set; } = string.Empty;
        
        // Additional Fields
        [StringLength(50)]
        public string Cartons { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string OrderQtyPac { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string OrderQtyPcs { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string ShipQtyPac { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string ShipQtyPcs { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string Destination { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Packing details are required")]
        [MinLength(1, ErrorMessage = "At least one packing detail is required")]
        public List<PackingDetailDto> PackingDetails { get; set; } = new();
        
        [Required(ErrorMessage = "Declaration answers are required")]
        [MinLength(1, ErrorMessage = "At least one declaration answer is required")]
        public List<DeclarationAnswerDto> DeclarationAnswers { get; set; } = new();
    }

    public class UpdatePackagingListDto
    {
        [Required(ErrorMessage = "Company is required")]
        [StringLength(100)]
        public string Company { get; set; } = string.Empty;

        [Required(ErrorMessage = "Buyer is required")]
        [StringLength(100)]
        public string Buyer { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string Style { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string PurchaseOrder { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string StyleBuyer { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string ShippedBy { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string ShortDescription { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string Size { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string Color { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string Brand { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string Packing { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string PcsPerSet { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string ProductNo { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string CountryOfOrigin { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string PortOfLoading { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string CountryOfDestination { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string PortOfDischarge { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string Incoterm { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string OrmsNo { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string LmkgNo { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string OrmsStyleNo { get; set; } = string.Empty;
        
        [StringLength(20)]
        public string Status { get; set; } = string.Empty;
        
        // Buyer Specific Fields
        [StringLength(50)]
        public string ItemNo { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string Pod { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string Boi { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string Wwwk { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string NoOfColor { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string KeyCode { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string SupplierCode { get; set; } = string.Empty;
        
        // Additional Fields
        [StringLength(50)]
        public string Cartons { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string OrderQtyPac { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string OrderQtyPcs { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string ShipQtyPac { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string ShipQtyPcs { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string Destination { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Packing details are required")]
        [MinLength(1, ErrorMessage = "At least one packing detail is required")]
        public List<PackingDetailDto> PackingDetails { get; set; } = new();

        [Required(ErrorMessage = "Declaration answers are required")]
        [MinLength(1, ErrorMessage = "At least one declaration answer is required")]
        public List<DeclarationAnswerDto> DeclarationAnswers { get; set; } = new();
        public string? OrmNo { get; set; }
    }
}